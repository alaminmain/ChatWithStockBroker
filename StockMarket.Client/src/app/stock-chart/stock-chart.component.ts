import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../company.service';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ApexAxisChartSeries, ApexChart, ApexXAxis, ApexYAxis, ApexTitleSubtitle, ApexDataLabels, ApexStroke, ApexTooltip, ApexPlotOptions } from 'ng-apexcharts';

// Define ChartOptions interface directly as a fallback
export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis | ApexYAxis[];
  title: ApexTitleSubtitle;
  dataLabels: ApexDataLabels;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
  plotOptions: ApexPlotOptions;
  // annotations: ApexAnnotations; // Removed
};

// Define ChartOptions interface if not already defined by ng-apexcharts
// export type ChartOptions = {
//   series: ApexAxisChartSeries;
//   chart: ApexChart;
//   xaxis: ApexXAxis;
//   yaxis: ApexYAxis | ApexYAxis[];
//   title: ApexTitleSubtitle;
//   dataLabels: ApexDataLabels;
//   stroke: ApexStroke;
//   tooltip: ApexTooltip;
//   plotOptions: ApexPlotOptions;
//   annotations: ApexAnnotations;
// };

@Component({
  selector: 'app-stock-chart',
  standalone: true,
  imports: [CommonModule, FormsModule, NgApexchartsModule],
  templateUrl: './stock-chart.component.html',
  styleUrl: './stock-chart.component.css'
})
export class StockChartComponent implements OnInit {
  searchQuery: string = '';
  companies: any[] = [];
  selectedCompany: any = null;
  marPriceData: any[] = [];

  public chartOptions: Partial<ChartOptions> | any; // Use Partial<ChartOptions> for flexibility

  constructor(private companyService: CompanyService) {
    this.chartOptions = {
      series: [{
        data: []
      }],
      chart: {
        type: 'candlestick',
        height: 350
      },
      title: {
        text: 'Candlestick Chart',
        align: 'left'
      },
      xaxis: {
        type: 'datetime'
      },
      yaxis: {
        tooltip: {
          enabled: true
        }
      }
    };
  }

  ngOnInit(): void {
  }

  onSearchCompany(): void {
    if (this.searchQuery.length > 2) { // Search only if query is at least 3 characters
      this.companyService.getCompanies(this.searchQuery, 1, 10, '', '')
        .subscribe(response => {
          this.companies = response.companies;
          this.selectedCompany = null; // Clear selected company
          this.marPriceData = []; // Clear previous chart data
          this.updateChartSeries([]); // Clear chart
        });
    } else {
      this.companies = [];
      this.selectedCompany = null;
      this.marPriceData = [];
      this.updateChartSeries([]);
    }
  }

  selectCompany(company: any): void {
    this.selectedCompany = company;
    this.companies = []; // Clear search results
    this.loadMarPriceData(company.compCd);
  }

  loadMarPriceData(compCd: number): void {
    this.companyService.getMarPriceData(compCd) // Assuming you add this method to CompanyService
      .subscribe(data => {
        this.marPriceData = data;
        this.updateChartSeries(data);
      }, error => {
        console.error('Error loading market price data:', error);
        this.marPriceData = [];
        this.updateChartSeries([]);
      });
  }

  updateChartSeries(data: any[]): void {
    const seriesData = data.map(item => {
      return {
        x: new Date(item.transDt),
        y: [item.open, item.high, item.low, item.close]
      };
    });

    this.chartOptions = {
      ...this.chartOptions,
      series: [{
        data: seriesData
      }],
      title: {
        text: this.selectedCompany ? `${this.selectedCompany.compNm} Candlestick Chart` : 'Candlestick Chart',
        align: 'left'
      }
    };
  }
}
