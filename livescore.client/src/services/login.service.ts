import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginService {
  private baseUrl = 'https://localhost:7245/api'

  constructor(private http: HttpClient) { }


  loginUser(credentials: { username: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/User/login`, credentials).pipe(catchError(this.handleError));
  }


  // Generic error handler
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client-side error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Server-side error: ${error.status} - ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }

  registerUser(data: { username: string; password: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/User/register`, data).pipe(catchError(this.handleError));
  }

  decodeJWT(token: string): any {
    try {
      // Vérifier si le token est valide (au moins deux points pour avoir trois parties)
      if (!token || token.split('.').length !== 3) {
        throw new Error('Token JWT invalide');
      }

      const payload = token.split('.')[1]; // La deuxième partie contient la payload encodée en Base64
      const decodedPayload = atob(payload); // Décoder la payload en chaîne lisible
      return JSON.parse(decodedPayload); // Convertir en objet JSON
    } catch (error) {
      console.error('Erreur lors du décodage du JWT :', error);
      return null;
    }
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


