import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SignalRService } from '../../services/signalr.service';
import Config from '../models/config'; // Importez votre modèle Config
import { ApiService } from '../../services/api.service';
import { TimerService } from '../../services/timer.service';

@Component({
  selector: 'app-viewer-match',
  standalone: false,
  templateUrl: './viewer-match.component.html',
  styleUrls: ['./viewer-match.component.scss'],
})
export class ViewerMatchComponent implements OnInit, OnDestroy {
  config: Config | any;
  // Propriétés principales
  chrono: any;
  matchId!: number;
  currentScore = { home: 0, away: 0 };
  currentQuarter = 1;
  equipeDomicile: any;
  equipeExterieur: any;
  joueursDomicile: any[] = [];
  joueursExterieur: any[] = [];
  selectedPlayerId: number | null = null;
  selectedVictimeId: number | null = null;
  selectedTypeFaute: string = 'P1';
  userId!: number;
  timeRemaining: number = 0;
  matchStatus = 'A venir';
  logs: string[] = [];
  quartTempsDuree: number = 0;
  timeoutActive: boolean = false;

  // Timeout
  timeoutRemaining: number = 0;
  dureeTimeout: number = 30;

  // Gestion des joueurs sur le terrain
  joueursDomicileSurTerrain: number[] = [];
  joueursExterieurSurTerrain: number[] = [];

  // Changements de joueur
  joueurSortantId: number = 0;
  joueurEntrantId: number = 0;

  cinqDeBase: any[] = []; // Joueurs sélectionnés pour le cinq de base
  joueursDisponibles: any[] = []; // Joueurs disponibles pour la configuration
  errorMessage = ''; // Message d'erreur pour l'utilisateur

 
  constructor(private route: ActivatedRoute,
    public signalRService: SignalRService,
    private router: Router,
    private apiService: ApiService,
    private timerService: TimerService,
    )
  { }

  ngOnInit(): void {
    // Récupération de l'ID du match
    this.matchId = parseInt(this.route.snapshot.paramMap.get('id') || '', 10);
  }

  loadMatchDetails(): void {


    this.apiService.getMatchDetailsWithPlayers(this.matchId).subscribe(
      (data) => {
        this.currentScore = {
          home: data.match.scoreDomicile || 0,
          away: data.match.scoreExterieur || 0,
        };
        this.currentQuarter = data.match.currentQuarter || 1;
        this.matchStatus = data.match.statut || 'A venir';
        this.equipeDomicile = data.match.equipeDomicile || {};
        this.equipeExterieur = data.match.equipeExterieur || {};
        this.joueursDomicile = this.equipeDomicile.joueurs || [];
        this.joueursExterieur = this.equipeExterieur.joueurs || [];

        if (data.match.chrono?.etat === 'Running') {
          this.timerService.startTimer(
            this.timerService.parseTimeStringToSeconds(data.match.chrono.tempsRestant)
          );
        } else {
          this.timerService.stopTimer();
        }
      },
      (error) => {
        console.error('Erreur lors du chargement des détails du match:', error);
        this.router.navigate(['/home']);
      }
    );
  }
  loadMatchEvents(): void {
    if (!this.matchId) {
      console.error('matchId non défini');
      return;
    }

    this.apiService.getEventsByMatchId(this.matchId).subscribe({
      next: (events: any[]) => {
        events.forEach((event) => {
          const eventString = this.formatEventToString(event);
          this.addLog(eventString);
        });
      },
      error: (err) => {
        console.error('Erreur lors de la récupération des événements :', err);
      },
    });
  }
  getMatchUpdates() {
    return this.signalRService.matchUpdates; // Retourne les événements en direct
  }

  ngOnDestroy(): void {
    // Quitter le groupe SignalR et stopper la connexion
    this.signalRService.stopConnection();
  }
  private addLog(message: string): void {
    this.logs.push(message);
  }

  private formatEventToString(event: any): string {
    switch (event.typeEvenement) {
      case 'Panier':
        return `J${event.joueur?.id} a marqué ${event.points} points pour ${event.equipe?.nom} à ${event.temps} (Quart-temps: ${event.quartTemps})`;
      case 'Faute':
        return `J${event.joueur?.id} a commis une faute de type ${event.typeFaute} sur J${event.joueurVictime?.nom ?? 'N/A'} à ${event.temps}`;
      case 'Changement':
        return `J${event.joueurSortantId} est sorti, remplacé par J${event.joueurEntrantId} à ${event.temps}`;
      case 'Timeout':
        return `${event.equipe?.id} a pris un timeout à ${event.temps}`;
      default:
        return `Événement inconnu : ${JSON.stringify(event)}`;
    }
  }



  public formatTime(seconds: number): string {
    if (seconds < 0) seconds = 0;
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${remainingSeconds
      .toString()
      .padStart(2, '0')}`;
  }


}

