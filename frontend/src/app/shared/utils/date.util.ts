export function openDatePicker(event: Event): void {
  const input = event.currentTarget as HTMLInputElement & { showPicker?: () => void };
  try { input.showPicker?.(); } catch {}
}

export function formatDate(value: string | null | undefined): string {
  if (!value) return '';
  const p = value.split('-');
  return p.length === 3 ? `${p[2]}/${p[1]}/${p[0]}` : '';
}
