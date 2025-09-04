import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompanyService } from '../company.service';

@Component({
  selector: 'app-company-detail-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './company-detail-modal.component.html',
  styleUrl: './company-detail-modal.component.css'
})
export class CompanyDetailModalComponent implements OnInit {
  @Input() compCd: number | null = null;
  @Output() close = new EventEmitter<void>();

  company: any;
  activeTab: string = 'basic'; // Default active tab

  constructor(private companyService: CompanyService) { }

  ngOnInit(): void {
    if (this.compCd) {
      this.companyService.getCompanyDetails(this.compCd)
        .subscribe(data => {
          this.company = data;
        });
    }
  }

  closeModal(): void {
    this.close.emit();
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}