import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { InformationsMatchesComponent } from './informations-matches/informations-matches.component';
import { EquipeComponent } from './equipe/equipe.component';
import { JoueurComponent } from './joueur/joueur.component';
import { ConfigdiversesComponent } from './configdiverses/configdiverses.component';
import { HomeComponent } from './home/home.component';
import { GameComponent } from './game/game.component';
import { ViewerMatchComponent } from './viewer-match/viewer-match.component';
import { RoleGuard } from './guards/role.guard';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
  {
    path: 'info-match',
    component: InformationsMatchesComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin', 'Preparateur'] },
  },
  {
    path: 'equipe',
    component: EquipeComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'joueur',
    component: JoueurComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'configdivers',
    component: ConfigdiversesComponent,
    canActivate: [RoleGuard],
    data: { roles: ['Admin', 'Preparateur'] },
  },
  { path: 'game/:id', component: GameComponent },
  { path: 'viewerMatch/:id', component: ViewerMatchComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
