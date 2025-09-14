import * as signalR from '@microsoft/signalr';

const URL = `${process.env.REACT_APP_API_BASE_URL}/chathub`;

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
