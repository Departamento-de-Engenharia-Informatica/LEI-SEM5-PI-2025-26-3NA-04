
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthService } from '@auth0/auth0-angular';

@Component({
  selector: 'app-create-user',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <h2>Create User</h2>
    <form (ngSubmit)="createUser()">
      <input [(ngModel)]="email" name="email" placeholder="Email" required />
      <input [(ngModel)]="name" name="name" placeholder="Name" required />
      <input [(ngModel)]="role" name="role" placeholder="Role" required />
      <button type="submit">Create</button>
    </form>
    <p>{{ message }}</p>
  `
})
export class CreateUser {
  email = '';
  name = '';
  role = '';
  message = '';

  constructor(private http: HttpClient, private auth: AuthService) {}

  createUser() {
    this.auth.getAccessTokenSilently().subscribe(token => {
      const url = `${environment.apiUrl}/auth/users`;
      const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      });

      this.http.post(url, { email: this.email, name: this.name, role: this.role }, { headers })
        .subscribe({
          next: () => this.message = 'User created successfully!',
          error: () => this.message = 'Error creating user.'
        });
    });
  }
}

