import * as signalR from '@microsoft/signalr';

const URL = 'http://localhost:5000/chathub'; // Assuming the API is running on port 5000

class SignalRService {
  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(URL)
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch(err => console.error('SignalR Connection Error: ', err));
  }

  onReceiveMessage(callback) {
    this.connection.on('ReceiveMessage', callback);
  }

  sendMessage(user, message) {
    this.connection.invoke('SendMessage', user, message).catch(err => console.error(err));
  }
}

const signalRService = new SignalRService();
export default signalRService;
