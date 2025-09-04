import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { provideAnimations } from '@angular/platform-browser/animations'; // Import provideAnimations

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), provideHttpClient(), AuthService, provideAnimations()] // Add provideAnimations()
};