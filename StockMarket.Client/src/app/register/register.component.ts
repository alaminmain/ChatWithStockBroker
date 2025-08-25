import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  model: any = {};

  constructor(private authService: AuthService, private router: Router) { }

  register() {
    this.authService.register(this.model).subscribe(() => {
      this.router.navigate(['/chat']);
    });
  }
}