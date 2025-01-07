import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import MatchEvent from '../app/models/match-event';
import { BehaviorSubject } from 'rxjs';
import { HubConnectionBuilder } from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  public hubConnection!: signalR.HubConnection;
  public matchUpdates = new BehaviorSubject<any>(null); // Liste des événements reçus en direct

  matchUpdate$ = this.matchUpdates.asObservable();

  constructor() {
    this.startConnection();

  }

  /**
   * Initialise et démarre la connexion SignalR
   */
  startConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:7245/Hubs/MatchHub') // URL de SignalR (ajustez selon vos besoins)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch((err) => console.error('Error while starting SignalR connection: ', err));


    this.hubConnection?.on('Receive', (matchId, message) => {
      this.matchUpdates.next({ matchId, message });
    })
    this.setupReconnection();
  }


  private setupReconnection(): void {
    this.hubConnection.onreconnecting((error) => {
      console.warn('SignalR reconnecting...', error);
    });

    this.hubConnection.onreconnected((connectionId) => {
      console.log('SignalR reconnected. Connection ID:', connectionId);
    });

    this.hubConnection.onclose((error) => {
      console.error('SignalR connection closed.', error);
      // Essayez de redémarrer la connexion après une fermeture
      setTimeout(() => this.startConnection(), 5000);
    });
  }

  /**
   * Arrête la connexion SignalR
   */
  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection
        .stop()
        .then(() => console.log('SignalR connection stopped'))
        .catch((err) => console.error('Error while stopping SignalR connection: ', err));
    }
  }
}
