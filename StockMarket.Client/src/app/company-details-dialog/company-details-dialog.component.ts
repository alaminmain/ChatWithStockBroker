import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CompanyService } from '../company.service';
import { CommonModule, DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-company-details-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    DatePipe
  ],
  templateUrl: './company-details-dialog.component.html',
  styleUrl: './company-details-dialog.component.css'
})
export class CompanyDetailsDialogComponent implements OnInit {
  company: any;
  activeTab: string = 'basic';

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { compCd: number },
    private companyService: CompanyService
  ) { }

  ngOnInit(): void {
    this.companyService.getCompanyDetails(this.data.compCd).subscribe(company => {
      this.company = company;
    });
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
