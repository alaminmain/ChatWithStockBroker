import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../environments/environment';

interface UserInfo {
  userName: string;
  // Add other user properties as needed
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn = new BehaviorSubject<boolean>(false);
  private currentUserSubject: BehaviorSubject<UserInfo | null>;
  public currentUserValue: UserInfo | null;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<UserInfo | null>(JSON.parse(localStorage.getItem('currentUser') || 'null'));
    this.currentUserValue = this.currentUserSubject.value;
    this.loggedIn.next(!!this.currentUserValue);
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  register(model: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/account/register`, model);
  }

  login(model: any): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/account/login`, model).pipe(
      tap((response: any) => {
        // Assuming the API returns user info on successful login
        const user: UserInfo = { userName: model.email }; // Replace with actual user info from response if available
        localStorage.setItem('currentUser', JSON.stringify(user));
        this.currentUserSubject.next(user);
        this.loggedIn.next(true);
      })
    );
  }

  logout(): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/account/logout`, {}).pipe(
      tap(() => {
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
        this.loggedIn.next(false);
      })
    );
  }
}