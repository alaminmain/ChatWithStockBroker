import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private apiUrl = 'https://localhost:7001/api/StockMarket'; // Adjust if your API URL is different

  constructor(private http: HttpClient) { }

  getCompanies(search: string = '', pageNumber: number = 1, pageSize: number = 10): Observable<any> {
    let params = new HttpParams();
    if (search) {
      params = params.append('search', search);
    }
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return this.http.get(`${this.apiUrl}/companies`, { params });
  }

  getCompanyDetails(compCd: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/companies/${compCd}`);
  }
}