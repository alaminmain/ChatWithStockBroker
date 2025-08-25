import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from './auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, CommonModule],
  template: `
    <nav class="main-header navbar navbar-expand navbar-white navbar-light">
      <ul class="navbar-nav">
        <li class="nav-item">
          <a class="nav-link" data-widget="pushmenu" href="#" role="button"><i class="fas fa-bars"></i></a>
        </li>
        <li class="nav-item d-none d-sm-inline-block">
          <a routerLink="/users" class="nav-link">Users</a>
        </li>
        <li class="nav-item d-none d-sm-inline-block">
          <a routerLink="/chat" class="nav-link">Chat</a>
        </li>
        <li class="nav-item d-none d-sm-inline-block" *ngIf="!(isLoggedIn$ | async)">
          <a routerLink="/login" class="nav-link">Login</a>
        </li>
        <li class="nav-item d-none d-sm-inline-block" *ngIf="!(isLoggedIn$ | async)">
          <a routerLink="/register" class="nav-link">Register</a>
        </li>
        <li class="nav-item d-none d-sm-inline-block" *ngIf="isLoggedIn$ | async">
          <a (click)="logout()" class="nav-link">Logout</a>
        </li>
      </ul>
    </nav>

    <aside class="main-sidebar sidebar-dark-primary elevation-4">
      <a href="#" class="brand-link">
        <span class="brand-text font-weight-light">Stock Market</span>
      </a>
      <div class="sidebar">
        <nav class="mt-2">
          <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview" role="menu" data-accordion="false">
            <li class="nav-item">
              <a routerLink="/users" class="nav-link">
                <i class="nav-icon fas fa-users"></i>
                <p>Users</p>
              </a>
            </li>
            <li class="nav-item">
              <a routerLink="/chat" class="nav-link">
                <i class="nav-icon fas fa-comments"></i>
                <p>Chat</p>
              </a>
            </li>
            <li class="nav-item" *ngIf="!(isLoggedIn$ | async)">
              <a routerLink="/login" class="nav-link">
                <i class="nav-icon fas fa-sign-in-alt"></i>
                <p>Login</p>
              </a>
            </li>
            <li class="nav-item" *ngIf="!(isLoggedIn$ | async)">
              <a routerLink="/register" class="nav-link">
                <i class="nav-icon fas fa-user-plus"></i>
                <p>Register</p>
              </a>
            </li>
            <li class="nav-item" *ngIf="isLoggedIn$ | async">
              <a (click)="logout()" class="nav-link">
                <i class="nav-icon fas fa-sign-out-alt"></i>
                <p>Logout</p>
              </a>
            </li>
          </ul>
        </nav>
      </div>
    </aside>

    <div class="content-wrapper">
      <section class="content">
        <div class="container-fluid">
          <router-outlet></router-outlet>
        </div>
      </section>
    </div>

    <footer class="main-footer">
      <div class="float-right d-none d-sm-block">
        <b>Version</b> 1.0.0
      </div>
      <strong>Copyright &copy; 2025 Stock Market.</strong> All rights reserved.
    </footer>
  `
})
export class AppComponent {
  isLoggedIn$ = this.authService.isLoggedIn();

  constructor(private authService: AuthService, private router: Router) {}

  logout() {
    this.authService.logout().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}