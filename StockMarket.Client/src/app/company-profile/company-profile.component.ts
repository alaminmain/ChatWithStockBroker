import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CompanyService } from '../company.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-company-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './company-profile.component.html',
  styleUrl: './company-profile.component.css'
})
export class CompanyProfileComponent implements OnInit {
  company: any;

  constructor(private route: ActivatedRoute, private companyService: CompanyService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const compCd = params.get('id');
      if (compCd) {
        this.companyService.getCompanyDetails(Number(compCd)).subscribe(data => {
          this.company = data;
        });
      }
    });
  }
}

