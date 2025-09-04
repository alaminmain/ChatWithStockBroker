
import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { UserListComponent } from './user-list/user-list.component';
import { ChatComponent } from './chat/chat.component';
import { AuthLayoutComponent } from './auth-layout/auth-layout.component';
import { MainLayoutComponent } from './main-layout/main-layout.component';
import { StockListComponent } from './stock-list/stock-list.component';
import { CompanyListComponent } from './company-list/company-list.component';
import { StockChartComponent } from './stock-chart/stock-chart.component'; // Import StockChartComponent

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'login', // Redirect to login by default
        pathMatch: 'full'
    },
    {
        path: '', // This path will use the AuthLayoutComponent for its children
        component: AuthLayoutComponent,
        children: [
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
        ]
    },
    {
        path: '', // This path will use the MainLayoutComponent for its children
        component: MainLayoutComponent,
        children: [
            { path: 'users', component: UserListComponent },
            { path: 'chat', component: ChatComponent },
            { path: 'stock-list', component: StockListComponent },
            { path: 'company-list', component: CompanyListComponent },
            { path: 'stock-chart', component: StockChartComponent }, // New route for stock chart
            { path: '', redirectTo: 'users', pathMatch: 'full' } // Redirect to users by default within the main layout
        ]
    }
];
