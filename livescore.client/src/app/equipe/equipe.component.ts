import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-equipe',
  standalone:false,
  templateUrl: './equipe.component.html',
  styleUrls: ['./equipe.component.scss']
})
export class EquipeComponent implements OnInit {
  teamForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.teamForm = this.fb.group({
      nom: ['', Validators.required],
      code: ['', Validators.required],
      coachNom: ['', Validators.required]
    });
  }

  submitTeam(): void {
    if (this.teamForm.valid) {
      const teamData = this.teamForm.value;

      const teamPayload = {
        nom: teamData.nom,
        code: teamData.code,
        estEquipeDomicile: false // Défaut, ajustez si nécessaire
      };

      this.apiService.addTeam(teamPayload).subscribe({
        next: (teamResponse) => {
          const coachPayload = {
            nom: teamData.coachNom,
            equipeId: teamResponse.id
          };

          this.apiService.addCoach(coachPayload).subscribe({
            next: () => {
              alert('Équipe et entraîneur créés avec succès !');
              this.router.navigate(['/home']); // Redirection après création
            },
            error: (error) => {
              console.error('Erreur lors de la création de l’entraîneur :', error);
              alert('L’équipe a été créée, mais une erreur s’est produite lors de l’ajout de l’entraîneur.');
            }
          });
        },
        error: (error) => {
          console.error('Erreur lors de la création de l’équipe :', error);
          alert('Erreur lors de la création de l’équipe.');
        }
      });
    } else {
      alert('Veuillez remplir tous les champs obligatoires.');
    }
  }

  logout(): void {
    localStorage.removeItem('username');
    this.router.navigate(['/login']);
  }
}
