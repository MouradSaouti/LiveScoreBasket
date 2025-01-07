import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import Config from '../models/config';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  matchConfigurations: Config[] = [];
  userRole: string = ''; // Rôle de l'utilisateur
  tokenjwt: string = '';

  constructor(private apiService: ApiService, private router: Router) { }

  ngOnInit(): void {
    this.userRole = localStorage.getItem('userRole') || '';
    this.tokenjwt = localStorage.getItem('jwtToken') || '';

    
    this.getConfigurations();
  }

  getConfigurations(): void {
    this.apiService.getConfigurations().subscribe(
      (data) => {
        this.matchConfigurations = data;
      },
      (error) => {
        console.error('Erreur lors de la récupération des configurations :', error);
      }
    );
  }

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userRole');
    localStorage.removeItem('jwtToken');
    this.router.navigate(['/login']);
  }

  player(): void {
    if (this.userRole === 'Admin') {
      this.router.navigate(['/joueur']);
    }
  }

  team(): void {
    if (this.userRole === 'Admin') {
      this.router.navigate(['/equipe']);
    }
  }

  config(): void {
    if (this.userRole === 'Admin' || this.userRole === 'Preparateur') {
      this.router.navigate(['/info-match']);
    }
  }

  startMatch(config: Config): void {
    this.router.navigate(['/game', config.id]);
  }

  viewMatch(config: Config): void {
    this.router.navigate(['/viewerMatch', config.id]);
  }
}
