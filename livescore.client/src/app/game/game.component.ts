import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { TimerService } from '../../services/timer.service';
import { Subscription } from 'rxjs';
import { SignalRService } from '../../services/signalr.service';

@Component({
  selector: 'app-game',
  standalone:false,
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss'],
})
export class GameComponent implements OnInit, OnDestroy {
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

  // Gestion des abonnements
  private subscriptions: Subscription[] = [];
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private timerService: TimerService,
    public signalRService: SignalRService,
  ) { }

  ngOnInit(): void {
    this.initializeUser();
    this.loadConfiguration();
    this.setupMatch();
    this.initializePlayersOnCourt();
    this.loadJoueursDisponibles();
    this.loadMatchEvents();
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
    this.timerService.stopTimer();
    this.signalRService.stopConnection();
  }

  private initializeUser(): void {
    const storedUserId = localStorage.getItem('authToken');
    this.userId = parseInt(storedUserId || '0', 10);
    if (isNaN(this.userId) || this.userId <= 0) {
      alert('Utilisateur non connecté ou ID utilisateur invalide.');
      this.router.navigate(['/login']);
    }
  }

  private loadConfiguration(): void {
    this.apiService.getConfigurations().subscribe(
      (config) => {
        this.quartTempsDuree = config.dureeQuartTemps || 10 * 60;
        this.dureeTimeout = config.dureeTimeout || 30;
      },
      (error) => {
        console.error('Erreur lors de la récupération de la configuration du match :', error);
      }
    );
  }

  private setupMatch(): void {
    this.route.params.subscribe((params) => {
      this.matchId = +params['id'];
      this.loadMatchDetails();
    });

    this.subscriptions.push(
      this.timerService.getTimeRemaining().subscribe((time) => {
        this.timeRemaining = time;
        if (time === 0) this.handleQuarterEnd();
      }),
      this.timerService.getCurrentQuarter().subscribe((quarter) => {
        this.currentQuarter = quarter;
      }),
      this.timerService.getMatchStatus().subscribe((status) => {
        this.matchStatus = status;
        if (status === 'Terminé') {
          this.timerService.stopTimer();
          this.addLog('Le match est terminé.');
        }
      })
    );
  }

  private initializePlayersOnCourt(): void {
    this.joueursDomicileSurTerrain = this.joueursDomicile.slice(0, 5).map((j) => j.id);
    this.joueursExterieurSurTerrain = this.joueursExterieur.slice(0, 5).map((j) => j.id);
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

        this.initializePlayersOnCourt();

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

  startMatch(): void {
    if (this.cinqDeBase.length < 5) {
      this.errorMessage = 'Vous devez sélectionner 5 joueurs pour le cinq de base avant de commencer le match.';
      return;
    }

    // Logique pour démarrer le match après la configuration du cinq de base
    this.startChrono();
  }


  startChrono(): void {
    if (this.loading) return;

    this.loading = true;

    this.timerService.startChrono(this.matchId).subscribe(
      (response) => {
        this.chrono = response;
        this.timerService.startTimer(this.quartTempsDuree);
        this.loading = false;
      },
      (error) => {
        console.error('Erreur lors du démarrage du chronomètre :', error);
        this.loading = false;
      }
    );
  }

  stopChrono(): void {
    if (this.loading) return;

    this.loading = true;
    this.timerService.stopChrono(this.matchId).subscribe(
      (response) => {
        this.chrono = response;
        this.timerService.stopTimer();
        this.loading = false;
      },
      (error) => {
        console.error('Erreur lors de l\'arrêt du chronomètre :', error);
        this.loading = false;
      }
    );
  }

  startTimeout(): void {
    if (this.timeRemaining <= 0) {
      alert('Le chrono principal est déjà arrêté. Vous ne pouvez pas demander un time-out.');
      return;
    }

    const timeoutDto = {
      matchId: this.matchId,
      typeEvenement: 'Timeout',
      temps: this.formatTime(this.timeRemaining),
      timestamp: new Date().toISOString(),
      equipeId: this.equipeDomicile.id,
      encodageUserId: this.userId,
    };

    this.apiService.enregistrerTimeout(timeoutDto).subscribe(
      () => {
        this.addLog(
          `Time-out demandé par l'équipe ${this.equipeDomicile.nom} à ${this.formatTime(this.timeRemaining)} (QT: ${this.currentQuarter}).`
        );

        // Désactiver les actions
        this.timeoutActive = true;

        // Arrêter le chrono principal
        this.timerService.stopTimer();
        this.timeoutRemaining = this.dureeTimeout;

        const timeoutInterval = setInterval(() => {
          this.timeoutRemaining--;
          if (this.timeoutRemaining <= 0) {
            clearInterval(timeoutInterval);
            alert('Le time-out est terminé.');

            // Réactiver les actions
            this.timeoutActive = false;

            // Reprendre le chrono principal
            this.timerService.startTimer(this.timeRemaining);
          }
        }, 1000);
      },
      (error) => {
        console.error('Erreur lors de la demande de time-out :', error);
        alert('Une erreur est survenue lors de la demande de time-out.');
      }
    );
  }


  private handleQuarterEnd(): void {
    if (this.currentQuarter < 4) {
      this.addLog(`Fin du quart-temps ${this.currentQuarter}`);
      this.currentQuarter++;
      this.timeRemaining = this.quartTempsDuree;
      this.timerService.startTimer(this.timeRemaining);
      this.addLog(`Début du quart-temps ${this.currentQuarter}`);
    } else {
      this.matchStatus = 'Terminé';
      this.timeRemaining = 0;
      this.addLog('Le match est terminé.');
      this.timerService.stopTimer();
    }
  }

  nextQuarter(): void {
    if (this.currentQuarter < 4) {
      this.handleQuarterEnd();
    } else {
      this.addLog('Vous êtes déjà au dernier quart-temps.');
    }
  }

  enregistrerFaute(): void {
    if (this.matchStatus === 'Terminé') {
      alert('Le match est terminé. Vous ne pouvez plus ajouter d\'événements.');
      return;
    }

    if (!this.selectedPlayerId || !this.selectedVictimeId) {
      alert('Veuillez sélectionner un joueur fautif et une victime.');
      return;
    }

    const fautif = this.findJoueurById(this.selectedPlayerId);
    const victime = this.findJoueurById(this.selectedVictimeId);

    const eventDto = {
      matchId: this.matchId,
      joueurId: this.selectedPlayerId,
      typeEvenement: 'Faute',
      typeFaute: this.selectedTypeFaute,
      temps: this.formatTime(this.timeRemaining),
      quartTemps: this.currentQuarter,
      equipeId: fautif?.equipeId,
      encodageUserId: this.userId,
      joueurVictimeId: this.selectedVictimeId,
    };

    this.apiService.enregistrerFaute(eventDto).subscribe(
      () => {
        this.addLog(
          `${fautif?.nom} a fait une faute (${this.selectedTypeFaute}) sur ${victime.nom} à ${this.formatTime(this.timeRemaining)} (QT: ${this.currentQuarter}).`
        );

        // Diffusion via SignalR
        this.signalRService.hubConnection.invoke('BroadcastEvent', this.matchId, eventDto)
          .catch(err => console.error('Error broadcasting event via SignalR:', err));

        this.resetSelections();
      },
      (error) => {
        console.error('Erreur lors de l\'enregistrement de la faute :', error);
      }
    );
  }


  updateScore(team: 'home' | 'away', points: number): void {
    if (this.matchStatus === 'Terminé') {
      alert('Le match est terminé. Vous ne pouvez plus ajouter d\'événements.');
      return;
    }

    if (!this.selectedPlayerId) {
      alert('Veuillez sélectionner un joueur avant d\'enregistrer un panier.');
      return;
    }

    const joueur = this.findJoueurById(this.selectedPlayerId);

    const eventDto = {
      matchId: this.matchId,
      joueurId: this.selectedPlayerId,
      typeEvenement: 'Panier',
      temps: this.formatTime(this.timeRemaining),
      quartTemps: this.currentQuarter,
      points: points,
      equipeId: team === 'home' ? this.equipeDomicile.id : this.equipeExterieur.id,
      encodageUserId: this.userId,
    };

    this.apiService.enregistrerPanierEtMettreAJourScore(eventDto).subscribe(
      () => {
        this.addLog(
          `${joueur?.nom} a marqué un panier de ${points} point(s) à ${this.formatTime(this.timeRemaining)} (QT: ${this.currentQuarter}).`
        );
        this.currentScore[team] += points;

        this.signalRService.hubConnection.invoke('BroadcastEvent', this.matchId, eventDto)
          .catch(err => console.error('Erreur lors de la diffusion de l\'événement via SignalR :', err));

        this.resetSelections();
      },
      (error) => {
        console.error('Erreur lors de l\'enregistrement du panier :', error);
      }
    );
  }


  changerJoueur(joueurSortantId: number | string, joueurEntrantId: number | string): void {
    if (this.matchStatus === 'Terminé') {
      alert('Le match est terminé. Vous ne pouvez plus ajouter d\'événements.');
      return;
    }

    const sortantId = typeof joueurSortantId === 'string' ? parseInt(joueurSortantId, 10) : joueurSortantId;
    const entrantId = typeof joueurEntrantId === 'string' ? parseInt(joueurEntrantId, 10) : joueurEntrantId;

    if (!sortantId || !entrantId) {
      alert('Veuillez sélectionner le joueur sortant et le joueur entrant.');
      return;
    }

    const joueurSortant = this.findJoueurById(sortantId);
    const joueurEntrant = this.findJoueurById(entrantId);

    if (!joueurSortant || !joueurEntrant) {
      alert('Joueur sortant ou entrant introuvable.');
      return;
    }

    if (joueurSortant.equipeId !== joueurEntrant.equipeId) {
      alert('Les joueurs doivent appartenir à la même équipe pour effectuer un changement.');
      return;
    }

    const changementDto = {
      matchId: this.matchId,
      joueurSortantId: sortantId,
      joueurEntrantId: entrantId,
      typeEvenement: 'Changement',
      temps: this.formatTime(this.timeRemaining),
      quartTemps: this.currentQuarter,
      encodageUserId: this.userId,
    };

    this.apiService.enregistrerChangement(changementDto).subscribe(
      () => {
        this.addLog(
          `Changement : ${joueurSortant.nom} (#${joueurSortant.numero}) sort et ${joueurEntrant.nom} (#${joueurEntrant.numero}) entre à ${this.formatTime(this.timeRemaining)} (QT: ${this.currentQuarter}).`
        );
        this.signalRService.hubConnection.invoke('BroadcastEvent', this.matchId, changementDto)
          .catch(err => console.error('Error broadcasting event via SignalR:', err));
      },
      (error) => {
        console.error('Erreur lors du changement de joueur :', error);
      }
    );
  }

  isPlayerOnCourt(joueurId: number): boolean {
    return (
      this.joueursDomicileSurTerrain.includes(joueurId) ||
      this.joueursExterieurSurTerrain.includes(joueurId)
    );
  }

  private findJoueurById(id: number): any {
    const joueurDomicile = this.joueursDomicile.find((j) => j.id === id);
    const joueurExterieur = this.joueursExterieur.find((j) => j.id === id);
    return joueurDomicile || joueurExterieur;
  }

  private addLog(message: string): void {
    this.logs.push(message);
  }

  private resetSelections(): void {
    this.selectedPlayerId = null;
    this.selectedVictimeId = null;
    this.selectedTypeFaute = 'P1';
  }
  onVictimeIdChange(value: any): void {
    this.selectedVictimeId = Number(value);
  }

  addPlayerToCinqDeBase(joueur: any): void {
    if (this.cinqDeBase.length >= 5) {
      this.errorMessage = 'Vous ne pouvez pas ajouter plus de 5 joueurs au cinq de base.';
      return;
    }

    this.apiService.addPlayerToCinqDeBase(this.matchId, joueur.equipeId, joueur.id).subscribe({
      next: () => {
        this.cinqDeBase.push(joueur); // Ajouter le joueur localement
        this.errorMessage = '';
      },
      error: (err) => {
        this.errorMessage = 'Erreur lors de l\'ajout du joueur au cinq de base.';
        console.error(err);
      },
    });
  }


  loadCinqDeBase(): void {
    this.apiService.getCinqDeBaseByMatchId(this.matchId).subscribe(
      (cinqDeBase) => {
        console.log('Cinq de base récupéré :', cinqDeBase);
        this.cinqDeBase = cinqDeBase; // Stockez les joueurs pour affichage
      },
      (error) => {
        console.error('Erreur lors de la récupération du cinq de base :', error);
      }
    );
  }


  loadJoueursDisponibles(): void {
    this.joueursDisponibles = [...this.joueursDomicile, ...this.joueursExterieur];
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
