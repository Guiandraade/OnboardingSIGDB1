import { FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Notification } from '../models/error.model';

function getStringValue(value: unknown): string | null {
  return typeof value === 'string' && value.trim().length > 0 ? value : null;
}

function normalizeNotification(item: unknown): Notification | null {
  if (!item || typeof item !== 'object') return null;

  const source = item as { [k: string]: unknown };
  const key = getStringValue(source['key']) ?? getStringValue(source['Key']) ?? '';
  const message = getStringValue(source['message']) ?? getStringValue(source['Message']);

  if (!message) return null;
  return { key, message };
}

function parseJsonString(value: string): unknown {
  const trimmed = value.trim();
  if (!trimmed) return value;

  if (!(trimmed.startsWith('{') || trimmed.startsWith('['))) {
    return value;
  }

  try {
    return JSON.parse(trimmed);
  } catch {
    return value;
  }
}

function unwrapPayload(errorPayload: unknown): unknown {
  if (typeof errorPayload === 'string') {
    return parseJsonString(errorPayload);
  }

  if (!errorPayload || typeof errorPayload !== 'object') {
    return errorPayload;
  }

  const payload = errorPayload as { [k: string]: unknown };

  const rawText = payload['text'];
  if (typeof rawText === 'string' && rawText.trim().length > 0) {
    return parseJsonString(rawText);
  }

  if ('error' in payload && payload['error'] !== undefined) {
    return unwrapPayload(payload['error']);
  }

  if ('Error' in payload && payload['Error'] !== undefined) {
    return unwrapPayload(payload['Error']);
  }

  return errorPayload;
}

export function extractNotifications(errorPayload: unknown): Notification[] {
  const unwrapped = unwrapPayload(errorPayload);
  if (!Array.isArray(unwrapped)) return [];

  return unwrapped
    .map((item) => normalizeNotification(item))
    .filter((item): item is Notification => !!item);
}

export function extractErrorMessages(errorPayload: unknown): string[] {
  const unwrapped = unwrapPayload(errorPayload);
  if (!unwrapped) return [];

  if (typeof unwrapped === 'string') {
    return unwrapped.trim() ? [unwrapped.trim()] : [];
  }

  const notifications = extractNotifications(unwrapped);
  if (notifications.length) {
    return notifications.map((n) => n.message);
  }

  if (typeof unwrapped === 'object') {
    const payload = unwrapped as { [k: string]: unknown };
    const messages: string[] = [];

    const directMessage = getStringValue(payload['message']) ?? getStringValue(payload['Message']);
    const title = getStringValue(payload['title']) ?? getStringValue(payload['Title']);
    const detail = getStringValue(payload['detail']) ?? getStringValue(payload['Detail']);

    if (directMessage) messages.push(directMessage);
    if (title) messages.push(title);
    if (detail) messages.push(detail);

    const errorsObj = payload['errors'] ?? payload['Errors'];
    if (errorsObj && typeof errorsObj === 'object') {
      Object.values(errorsObj as { [k: string]: unknown }).forEach((value) => {
        if (Array.isArray(value)) {
          value.forEach((entry) => {
            const text = getStringValue(entry);
            if (text) messages.push(text);
          });
          return;
        }

        const text = getStringValue(value);
        if (text) messages.push(text);
      });
    }

    return Array.from(new Set(messages));
  }

  return [];
}

export function applyNotificationsToForm(
  form: FormGroup,
  notifications: Notification[],
  keyMap: { [key: string]: string }
): string[] {
  const unmapped: string[] = [];

  for (const n of notifications) {
    const controlName = keyMap[(n.key ?? '').toLowerCase()];
    const ctrl = controlName ? form.get(controlName) : null;
    if (ctrl) {
      ctrl.setErrors({ ...(ctrl.errors ?? {}), serverError: n.message });
      ctrl.markAsTouched();
      continue;
    }
    unmapped.push(n.message);
  }

  return unmapped;
}

export function buildFormErrorMessage(err: HttpErrorResponse): string {
  const messages = extractErrorMessages(err?.error ?? err);
  if (messages.length) return messages.join('\n');
  if (err?.status > 0) {
    return err.statusText?.trim()
      ? `HTTP ${err.status} - ${err.statusText.trim()}`
      : `HTTP ${err.status}`;
  }
  return err?.message?.trim() || 'Request failed.';
}
