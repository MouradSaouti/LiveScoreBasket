import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-joueur',
  standalone:false,
  templateUrl: './joueur.component.html',
  styleUrls: ['./joueur.component.scss']
})
export class JoueurComponent implements OnInit {
  formJoueur: FormGroup;
  teams: any[] = [];
  selectedTeamId: number | null = null;
  players: any[] = [];

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router
  ) {
    this.formJoueur = this.fb.group({
      nom: ['', Validators.required],
      prenom: ['', Validators.required],
      taille: ['', Validators.required],
      numero: ['', Validators.required],
      position: ['', Validators.required],
      estCapitaine: [false],
      estEnJeu: [false],
    });
  }

  ngOnInit(): void {
    this.loadTeams();
  }

  // Charger les équipes depuis l'API
  loadTeams(): void {
    this.apiService.getTeams().subscribe(
      (teams) => {
        this.teams = teams;
      },
      (error) => {
        console.error('Erreur lors du chargement des équipes :', error);
        alert('Impossible de charger les équipes.');
      }
    );
  }

  // Ajouter un joueur à l'équipe sélectionnée
  submitPlayer(): void {
    if (!this.selectedTeamId) {
      alert('Veuillez sélectionner une équipe.');
      return;
    }

    if (this.formJoueur.invalid) {
      alert('Formulaire invalide.');
      return;
    }

    const playerData = { ...this.formJoueur.value, equipeId: this.selectedTeamId };

    // Vérifiez si un capitaine existe déjà
    if (playerData.estCapitaine && this.players.some(player => player.estCapitaine)) {
      alert('Cette équipe a déjà un capitaine.');
      return;
    }

    this.players.push(playerData); // Ajoute localement
    this.formJoueur.reset({ estCapitaine: false, estEnJeu: false }); // Réinitialise le formulaire
  }

  // Supprimer un joueur
  removePlayer(index: number): void {
    this.players.splice(index, 1);
  }

  // Soumettre tous les joueurs
  submitAllPlayers(): void {
    if (this.players.length === 0) {
      alert('Aucun joueur à enregistrer.');
      return;
    }

    let requestsCompleted = 0;

    this.players.forEach((player) => {
      this.apiService.addPlayer(player).subscribe({
        next: () => {
          requestsCompleted++;
          if (requestsCompleted === this.players.length) {
            alert('Tous les joueurs ont été enregistrés avec succès.');
            this.router.navigate(['/home']);
          }
        },
        error: () => {
          alert('Une erreur est survenue lors de l’enregistrement des joueurs.');
        }
      });
    });
  }

  logout(): void {
    localStorage.removeItem('username');
    this.router.navigate(['/login']);
  }
}
