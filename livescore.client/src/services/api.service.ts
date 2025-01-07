import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { LoginService } from './login.service'; // Importer LoginService

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://localhost:7245/api'
  constructor(private http: HttpClient, private loginService: LoginService) { }
  // ConfigurationsMatch Endpoints
  getConfigurations(): Observable<any> {
    return this.http.get(`${this.baseUrl}/ConfigurationsMatch`).pipe(catchError(this.handleError));
  }

  getConfigurationById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/ConfigurationsMatch/${id}`).pipe(catchError(this.handleError));
  }

  createConfigMatch(dto: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/ConfigurationsMatch`, dto).pipe(
      catchError(this.handleError)
    );
  }

  updateConfiguration(id: number, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/ConfigurationsMatch/${id}`, data).pipe(catchError(this.handleError));
  }

  deleteConfiguration(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/ConfigurationsMatch/${id}`).pipe(catchError(this.handleError));
  }

  ajouterEquipes(configId: number, equipes: { EquipeDomicileId: number | null, EquipeExterieurId: number | null }): Observable<any> {
    return this.http.post(`${this.baseUrl}/ConfigurationMatch/AjouterEquipes/${configId}`, equipes);
  }


  // Equipes Endpoints
  getTeams(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Equipes`).pipe(catchError(this.handleError));
  }

  getTeamById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Equipes/${id}`).pipe(catchError(this.handleError));
  }

  addTeam(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Equipes`, data).pipe(catchError(this.handleError));
  }

  deleteTeam(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Equipes/${id}`).pipe(catchError(this.handleError));
  }


  // Cinq de base endpoints 
  getCinqDeBaseByMatchId(matchId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/CinqDeBase/${matchId}`).pipe(
      catchError(this.handleError) // Ajout de la gestion des erreurs
    );
  }

  addPlayerToCinqDeBase(matchId: number, equipeId: number, joueurId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/CinqDeBase`, {
      matchId,
      equipeId,
      joueurId,
    }).pipe(catchError(this.handleError)); // Ajouter la gestion des erreurs si nécessaire
  }


  // Coach Endpoints
  getCoaches(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Coach`).pipe(catchError(this.handleError));
  }

  getCoachById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Coach/${id}`).pipe(catchError(this.handleError));
  }

  addCoach(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Coach`, data).pipe(catchError(this.handleError));
  }

  deleteCoach(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Coach/${id}`).pipe(catchError(this.handleError));
  }

  // Joueur Endpoints
  getPlayers(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Joueur`).pipe(catchError(this.handleError));
  }

  getPlayerById(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Joueur/${id}`).pipe(catchError(this.handleError));
  }

  addPlayer(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Joueur`, data).pipe(catchError(this.handleError));
  }

  deletePlayer(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Joueur/${id}`).pipe(catchError(this.handleError));
  }
  updatePlayer(id: number, data: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Joueur/${id}`, data).pipe(catchError(this.handleError));
  }

  // Fetch match details
  getMatchDetails(matchId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Matchs/${matchId}`);
  }
  getMatchDetailsWithPlayers(matchId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Matchs/${matchId}/details`);
  }

  checkMatch(configId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/ConfigurationsMatch/CheckMatch/${configId}`).pipe(
      catchError(this.handleError)
    );
  }

  // Update scores
  updateScore(dto: { matchId: number; scoreDomicile: number; scoreExterieur: number }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Matchs/score`, dto);
  }

  // Update quarters
  updateQuarter(dto: { matchId: number; quarter: number }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Matchs/quarter`, dto);
  }

  // Call timeout
  callTimeout(dto: { matchId: number; teamId: number; time: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Matchs/timeout`, dto);
  }

  //end point evenementsMatch

  //changement
  recordSubstitution(eventDto: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/EvenementsMatch/changement`, eventDto);
  }

  getEventsByMatchId(matchId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/EvenementsMatch/${matchId}`);
  }

  enregistrerPanierEtMettreAJourScore(eventDto: {
    matchId: number;
    joueurId: number;
    typeEvenement: string;
    temps: string;
    points: number;
    equipeId: number;
    encodageUserId: number;
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Matchs/enregistrer-panier`, eventDto).pipe(catchError(this.handleError));
  }

  enregistrerFaute(eventDto: {
    matchId: number;
    joueurId: number;
    typeEvenement: string;
    typeFaute: string;
    temps: string;
    equipeId: number;
    encodageUserId: number;
    joueurVictimeId?: number; // Optionnel
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/EvenementsMatch/faute`, eventDto).pipe(catchError(this.handleError));
  }

  enregistrerChangement(changementDto: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/EvenementsMatch/changement`, changementDto).pipe(
      catchError((error) => {
        console.error('Erreur API lors du changement de joueur :', error);
        return throwError(() => new Error('Erreur API lors du changement de joueur.'));
      })
    );
  }


  enregistrerTimeout(timeoutDto: any): Observable<any> {
    return this.http
      .post(`${this.baseUrl}/EvenementsMatch/timeout`, timeoutDto)
      .pipe(
        catchError((error) => {
          console.error('Erreur API lors de la demande de timeout :', error);
          throw error;
        })
      );
  }


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
}
