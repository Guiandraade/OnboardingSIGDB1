import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class UserService {
  readonly name = 'Guilherme Nascimento';
  readonly firstName = 'Guilherme';
  readonly role = 'Administrator';
  readonly avatarUrl = 'assets/1777576099585.jpg';
}
