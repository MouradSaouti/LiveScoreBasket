import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../../services/login.service';
import { HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  loginForm: FormGroup;
  registerForm: FormGroup;
  isLoading = false;
  error = '';
  isLoginMode = true; // Permet de basculer entre le mode Connexion et Inscription

  constructor(
    private fb: FormBuilder,
    private loginService: LoginService,
    private router: Router
  ) {
    // Formulaire de connexion
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });

    // Formulaire d'inscription
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onLogin(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.loginService.loginUser(this.loginForm.value).subscribe({
        next: (response: any) => {
          if (response?.token) {
            const decodedToken = this.decodeJWT(response.token);
            if (decodedToken.id && decodedToken.role) {
              localStorage.setItem('jwtToken', response.token);
              localStorage.setItem('authToken', decodedToken.id);
              localStorage.setItem('userRole', decodedToken.role);
              this.router.navigate(['/home']);
            } else {
              this.error = 'Token JWT incomplet';
            }
          }
        },
        error: (error) => {
          this.error = 'Identifiants incorrects';
          this.isLoading = false;
        }
      });
    }
  }


  onRegister(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.error = '';

      this.loginService.registerUser(this.registerForm.value).subscribe({
        next: () => {
          this.isLoginMode = true; // Retourner en mode Connexion après l'inscription
          this.error = 'Compte créé avec succès. Connectez-vous.';
        },
        error: (error) => {
          this.error = 'Erreur lors de la création du compte.';
          this.isLoading = false;
          console.error('Erreur lors de l\'inscription :', error);
        },
        complete: () => (this.isLoading = false),
      });
    } else {
      this.error = 'Veuillez remplir correctement tous les champs';
      Object.keys(this.registerForm.controls).forEach((key) => {
        const control = this.registerForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
    }
  }

  decodeJWT(token: string): any {
    try {
      const payload = token.split('.')[1]; // La deuxième partie contient la payload encodée en Base64
      const decodedPayload = atob(payload); // Décoder la payload en chaîne lisible
      return JSON.parse(decodedPayload); // Convertir en objet JSON
    } catch (error) {
      console.error('Erreur lors du décodage du JWT :', error);
      return null;
    }
  }
  isUserInRole(role: string): boolean {
    const userRole = localStorage.getItem('userRole');
    return userRole === role;
  }

  toggleMode(): void {
    this.isLoginMode = !this.isLoginMode;
    this.error = ''; // Réinitialiser les erreurs à chaque changement de mode
  }

  getUserRole(): string {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      const payload = token.split('.')[1];
      const decodedPayload = atob(payload);
      const parsedPayload = JSON.parse(decodedPayload);
      return parsedPayload?.role || '';
    }
    return '';
  }

  getHeaders(): HttpHeaders {
    const token = localStorage.getItem('jwtToken');
    if (token) {
      console.log('JWT envoyé dans les headers :', token);
    } else {
      console.error('Aucun JWT trouvé dans le localStorage.');
    }
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });
  }
}
