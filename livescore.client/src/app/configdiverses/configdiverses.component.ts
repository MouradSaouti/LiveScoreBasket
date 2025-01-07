import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-config-divers',
  standalone:false,
  templateUrl: './configdiverses.component.html',
  styleUrls: ['./configdiverses.component.scss']
})
export class ConfigdiversesComponent {
  constructor(private router: Router,) { }
  homeTeamNumber = 321;
  visitorTeamNumber = 123;

  invertTeams() {
    // Inverser les équipes et les flèches
    const temp = this.homeTeamNumber;
    this.homeTeamNumber = this.visitorTeamNumber;
    this.visitorTeamNumber = temp;
  }
  saveConfig() {
    console.log('Configuration enregistrée :', {
    });

    // Redirection vers /home
    this.router.navigate(['/home']);
  }
}
