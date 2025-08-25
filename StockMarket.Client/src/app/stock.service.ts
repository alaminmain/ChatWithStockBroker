
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Stock } from './stock.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StockService {
  private hubConnection: HubConnection;
  private stockUpdatesSubject = new Subject<Stock[]>();

  stockUpdates$ = this.stockUpdatesSubject.asObservable();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/stockhub`)
      .build();

    this.hubConnection.on('ReceiveStockUpdate', (stocks: Stock[]) => {
      this.stockUpdatesSubject.next(stocks);
    });

    this.hubConnection.start()
      .then(() => {
        console.log('Connection started!');
        this.hubConnection.invoke('GetInitialStocks');
      })
      .catch(err => console.error(err));
  }
}
