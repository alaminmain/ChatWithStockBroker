
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WatchlistService {
  private apiUrl = 'https://localhost:7108/api/watchlist';

  constructor(private http: HttpClient) { }

  getWatchlist(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  addToWatchlist(compId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${compId}`, {});
  }

  removeFromWatchlist(compId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${compId}`);
  }
}
