import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  private hubConnection: HubConnection;
  messages: { user: string, message: string, timestamp: Date }[] = [];
  message: string = '';

  constructor(private authService: AuthService) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/chathub`)
      .build();

    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      this.messages.push({ user, message, timestamp: new Date() });
    });

    this.hubConnection.start().catch(err => console.error(err));
  }

  ngOnInit(): void {
  }

  sendMessage(): void {
    this.authService.isLoggedIn().subscribe(isLoggedIn => {
      if (isLoggedIn) {
        this.hubConnection.invoke('SendMessage', this.authService.currentUserValue?.userName, this.message)
          .catch(err => console.error(err));
        this.message = '';
      } else {
        console.warn('User not logged in. Cannot send message.');
      }
    });
  }
}