import React, { useState, useEffect } from 'react';
import signalRService from './signalRService';

const Chat = () => {
  const [user, setUser] = useState('');
  const [message, setMessage] = useState('');
  const [messages, setMessages] = useState([]);

  useEffect(() => {
    signalRService.onReceiveMessage((user, message) => {
      setMessages(prevMessages => [...prevMessages, { user, message }]);
    });
  }, []);

  const handleSendMessage = () => {
    if (user && message) {
      signalRService.sendMessage(user, message);
      setMessage('');
    }
  };

  return (
    <div className="container mt-4">
      <h2>Chat</h2>
      <div className="mb-3">
        <input
          type="text"
          className="form-control"
          placeholder="Enter your name"
          value={user}
          onChange={(e) => setUser(e.target.value)}
        />
      </div>
      <div className="mb-3">
        <textarea
          className="form-control"
          placeholder="Enter your message"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        ></textarea>
      </div>
      <button className="btn btn-primary" onClick={handleSendMessage}>Send</button>

      <div className="mt-4">
        {messages.map((msg, index) => (
          <div key={index}>
            <strong>{msg.user}:</strong> {msg.message}
          </div>
        ))}
      </div>
    </div>
  );
};

export default Chat;
