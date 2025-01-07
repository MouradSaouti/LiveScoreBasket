import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, interval, Observable, Subscription } from 'rxjs';
import { catchError, map, takeWhile } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class TimerService {
  private baseUrl = 'https://localhost:7245/api/ChronoMatch';

  private timeRemaining$ = new BehaviorSubject<number>(0); // Temps restant en secondes
  private currentQuarter$ = new BehaviorSubject<number>(1); // Quart-temps actuel
  private matchStatus$ = new BehaviorSubject<string>('A venir'); // Statut du match

  private intervalSub: Subscription | undefined;

  constructor(private http: HttpClient) { }

  /**
   * Démarrer le chronomètre sur le backend et initialiser le timer local
   */
  startChrono(matchId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/start/${matchId}`, {}).pipe(
      map((response: any) => {
        const timeInSeconds =
          typeof response.tempsRestant === 'string'
            ? this.parseTimeStringToSeconds(response.tempsRestant)
            : response.tempsRestant;
        this.timeRemaining$.next(timeInSeconds); // Temps restant en secondes
        this.matchStatus$.next(response.etat); // État du chronomètre (Running)
        return response;
      }),
      catchError((error) => {
        console.error('Erreur lors du démarrage du chronomètre:', error);
        throw error;
      })
    );
  }

  /**
   * Arrêter le chronomètre sur le backend et arrêter le timer local
   */
  stopChrono(matchId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/stop/${matchId}`, {}).pipe(
      map((response: any) => {
        const timeInSeconds =
          typeof response.tempsRestant === 'string'
            ? this.parseTimeStringToSeconds(response.tempsRestant)
            : response.tempsRestant;
        this.timeRemaining$.next(timeInSeconds); // Temps restant
        this.matchStatus$.next(response.etat); // État (Stopped)
        this.stopTimer(); // Arrêter le timer local
        return response;
      }),
      catchError((error) => {
        console.error('Erreur lors de l\'arrêt du chronomètre:', error);
        throw error;
      })
    );
  }

  /**
   * Synchroniser le timer avec le backend
   */
  syncTimer(matchId: number): void {
    this.http.post(`${this.baseUrl}/update-chrono/${matchId}`, {}).subscribe(
      (data: any) => {
        const timeInSeconds =
          typeof data.tempsRestant === 'string'
            ? this.parseTimeStringToSeconds(data.tempsRestant)
            : data.tempsRestant;
        this.timeRemaining$.next(timeInSeconds); // Temps restant en secondes
        this.currentQuarter$.next(data.quartTempsActuel); // Quart-temps actuel
        this.matchStatus$.next(data.statutMatch); // Statut du match
        if (data.statutMatch === 'Terminé') {
          this.stopTimer(); // Arrêter le timer local si le match est terminé
        }
      },
      (error) => {
        console.error('Erreur lors de la synchronisation du timer:', error);
      }
    );
  }

  /**
   * Méthode utilitaire : Convertir une chaîne de type "MM:SS" en secondes
   */
  parseTimeStringToSeconds(timeString: string): number {
    if (typeof timeString !== 'string') {
      console.error('Format de temps invalide :', timeString);
      return 0;
    }

    // Découper la chaîne en parties HH:MM:SS
    const parts = timeString.split(':').map(Number);

    // Si la chaîne contient 3 parties (HH:MM:SS)
    if (parts.length === 3) {
      const [hours, minutes, seconds] = parts;
      return (hours * 3600) + (minutes * 60) + seconds;
    }

    // Format inattendu
    console.error('Format inattendu pour timeString :', timeString);
    return 0;
  }



  /**
   * Démarrer un timer local
   */
  startTimer(initialTime: number): void {
    if (this.intervalSub) {
      console.warn('Le chronomètre est déjà démarré.');
      return; // Évite une nouvelle initialisation
    }

    this.stopTimer(); // Arrête tout autre timer existant
    this.timeRemaining$.next(initialTime);

    this.intervalSub = interval(1000)
      .pipe(
        map(() => this.timeRemaining$.value - 1), // Décrémente chaque seconde
        takeWhile((time) => time >= 0) // Continue tant que le temps restant est >= 0
      )
      .subscribe((time) => {
        this.timeRemaining$.next(time);
        if (time === 0) {
          this.handleQuarterEnd(); // Gérer la fin du quart-temps
        }
      });
  }



  /**
   * Arrêter le timer local
   */
  stopTimer(): void {
    if (this.intervalSub) {
      this.intervalSub.unsubscribe();
      this.intervalSub = undefined;
    }
  }

  /**
   * Gérer la fin d'un quart-temps
   */
  private handleQuarterEnd(): void {
    const currentQuarter = this.currentQuarter$.value;
    if (currentQuarter < 4) {
      this.currentQuarter$.next(currentQuarter + 1); // Passer au quart-temps suivant
    } else {
      this.matchStatus$.next('Terminé'); // Terminer le match
      this.stopTimer();
    }
  }

  /**
   * Obtenir un observable du temps restant
   */
  getTimeRemaining(): Observable<number> {
    return this.timeRemaining$.asObservable();
  }

  /**
   * Obtenir un observable du quart-temps actuel
   */
  getCurrentQuarter(): Observable<number> {
    return this.currentQuarter$.asObservable();
  }

  /**
   * Obtenir un observable du statut du match
   */
  getMatchStatus(): Observable<string> {
    return this.matchStatus$.asObservable();
  }
}
