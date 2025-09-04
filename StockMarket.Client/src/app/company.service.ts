import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private apiUrl = `${environment.apiUrl}/api/StockMarket`;

  constructor(private http: HttpClient) { }

  getCompanies(
    search: string = '',
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy: string = '',
    sortDirection: string = ''
  ): Observable<any> {
    let params = new HttpParams();
    if (search) {
      params = params.append('search', search);
    }
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    if (sortBy) {
      params = params.append('sortBy', sortBy);
    }
    if (sortDirection) {
      params = params.append('sortDirection', sortDirection);
    }

    return this.http.get(`${this.apiUrl}/companies`, { params });
  }

  getCompanyDetails(compCd: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/companies/${compCd}`);
  }

  getMarPriceData(compCd: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/marprice/${compCd}`);
  }
}