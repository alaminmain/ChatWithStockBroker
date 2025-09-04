import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CompanyDetailModalComponent } from '../company-detail-modal/company-detail-modal.component';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, CompanyDetailModalComponent],
  templateUrl: './company-list.component.html',
  styleUrl: './company-list.component.css'
})
export class CompanyListComponent implements OnInit {
  companies: any[] = [];
  totalCount: number = 0;
  pageNumber: number = 1;
  pageSize: number = 10;
  search: string = '';
  selectedCompanyCompCd: number | null = null;

  constructor(private companyService: CompanyService) { }

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.companyService.getCompanies(this.search, this.pageNumber, this.pageSize)
      .subscribe(response => {
        this.companies = response.Companies;
        this.totalCount = response.TotalCount;
      });
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadCompanies();
  }

  onSearch(): void {
    this.pageNumber = 1; // Reset to first page on new search
    this.loadCompanies();
  }

  openCompanyDetails(compCd: number): void {
    this.selectedCompanyCompCd = compCd;
  }

  closeCompanyDetails(): void {
    this.selectedCompanyCompCd = null;
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}