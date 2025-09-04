
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterModule, CommonModule],
  template: `
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
      <a class="navbar-brand" href="#">Stock Market</a>
      <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav mr-auto">
          <li class="nav-item">
            <a class="nav-link" routerLink="/users">Users</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" routerLink="/chat">Chat</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" routerLink="/stock-list">Stock Prices</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" routerLink="/company-list">Companies</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" routerLink="/stock-chart">Stock Chart</a>
          </li>
        </ul>
        <ul class="navbar-nav">
          <li class="nav-item" *ngIf="!(isLoggedIn$ | async)">
            <a class="nav-link" routerLink="/login">Login</a>
          </li>
          <li class="nav-item" *ngIf="!(isLoggedIn$ | async)">
            <a class="nav-link" routerLink="/register">Register</a>
          </li>
          <li class="nav-item" *ngIf="isLoggedIn$ | async">
            <a class="nav-link" (click)="logout()">Logout</a>
          </li>
        </ul>
      </div>
    </nav>

    <div class="container mt-3">
      <router-outlet></router-outlet>
    </div>
  `
})
export class MainLayoutComponent implements OnInit {
  isLoggedIn$ = this.authService.isLoggedIn();

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
  }

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
