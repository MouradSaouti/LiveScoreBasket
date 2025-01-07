import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-informations-matches',
  standalone:false,
  templateUrl: './informations-matches.component.html',
  styleUrls: ['./informations-matches.component.scss']
})
export class InformationsMatchesComponent implements OnInit {
  matchForm!: FormGroup;
  teams: any[] = [];
  currentUserId: number = 1;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadTeams();
    this.initializeForm();
  }

  loadTeams(): void {
    this.apiService.getTeams().subscribe(
      (data) => {
        this.teams = data;
        console.log('Équipes chargées :', data);
      },
      (error) => {
        console.error('Erreur lors du chargement des équipes :', error);
        alert('Impossible de charger les équipes.');
      }
    );
  }

  initializeForm(): void {
    this.matchForm = this.fb.group({
      nomMatch: ['', Validators.required],
      dateHeure: ['', Validators.required],
      nombreQuartTemps: [4, [Validators.required, Validators.min(1)]],
      dureeQuartTemps: [12, [Validators.required, Validators.min(1)]],
      dureeTimeout: [60, [Validators.required, Validators.min(1)]],
      equipeDomicileId: [null, Validators.required],
      equipeExterieurId: [null, Validators.required],
    });
  }

  createConfiguration(): void {
    if (this.matchForm.invalid) {
      alert('Formulaire invalide. Veuillez corriger les erreurs.');
      return;
    }

    const formValue = this.matchForm.value;
    const dto = {
      ...formValue,
      userId: this.currentUserId,
      dureeQuartTemps: `00:${formValue.dureeQuartTemps}:00`,
      dureeTimeout: `00:00:${formValue.dureeTimeout}`,
    };

    this.apiService.createConfigMatch(dto).subscribe(
      (response) => {
        alert('Configuration créée avec succès !');
        this.nextPage();
      },
      (error) => {
        console.error('Erreur lors de la création de la configuration :', error);
        alert('Échec de la création.');
      }
    );
  }

  nextPage(): void {
    this.router.navigate(['/home']);
  }

  logout(): void {
    localStorage.removeItem('authToken');
    this.router.navigate(['/login']);
  }
}
