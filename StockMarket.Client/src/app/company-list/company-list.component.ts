import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CompanyDetailModalComponent } from '../company-detail-modal/company-detail-modal.component';
import { AgGridModule } from 'ag-grid-angular';
import { ColDef, CellClickedEvent, ModuleRegistry, AllCommunityModule } from 'ag-grid-community';

ModuleRegistry.registerModules([ AllCommunityModule ]);

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, CompanyDetailModalComponent, AgGridModule],
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

  colDefs: ColDef[] = [
    { field: 'compCd', headerName: 'Company Code' },
    { field: 'compNm', headerName: 'Company Name', onCellClicked: (event) => this.onCellClicked(event) },
    { field: 'athoCap', headerName: 'Authorized Capital' },
    { field: 'paidCap', headerName: 'Paid-up Capital' },
    { field: 'regOff', headerName: 'Registered Office' },
    { field: 'noShrs', headerName: 'Number of Shares' },
    { field: 'instrCd', headerName: 'Instrument Code' },
    { field: 'startDt', headerName: 'Start Date' },
    { field: 'fcVal', headerName: 'Face Value' }
  ];

  constructor(private companyService: CompanyService) { }

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.companyService.getCompanies(this.search, this.pageNumber, this.pageSize)
      .subscribe(response => {
        this.companies = response.companies;
        this.totalCount = response.totalCount;
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

  onCellClicked(event: CellClickedEvent): void {
    this.openCompanyDetails(event.data.compCd);
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }
}