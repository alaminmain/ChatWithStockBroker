import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="container mt-5">
      <router-outlet></router-outlet>
    </div>
  `,
  styleUrls: ['./auth-layout.component.css']
})
export class AuthLayoutComponent {

}