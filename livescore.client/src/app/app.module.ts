import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from './login/login.component';
import { InformationsMatchesComponent } from './informations-matches/informations-matches.component';
import { EquipeComponent } from './equipe/equipe.component';
import { JoueurComponent } from './joueur/joueur.component';
import { ConfigdiversesComponent } from './configdiverses/configdiverses.component';
import { HomeComponent } from './home/home.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { GameComponent } from './game/game.component';
import { ViewerMatchComponent } from './viewer-match/viewer-match.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    InformationsMatchesComponent,
    EquipeComponent,
    JoueurComponent,
    ConfigdiversesComponent,
    HomeComponent,
    GameComponent,
    ViewerMatchComponent
  ],
  imports: [
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatGridListModule,
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
