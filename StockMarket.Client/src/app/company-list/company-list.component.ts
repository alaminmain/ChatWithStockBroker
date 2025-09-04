import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { CompanyService } from '../company.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgApexchartsModule } from 'ng-apexcharts'; // Import NgApexchartsModule

import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { merge, of as observableOf } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';
import { MatDialog, MatDialogModule } from '@angular/material/dialog'; // Import MatDialog and MatDialogModule
import { CompanyDetailsDialogComponent } from '../company-details-dialog/company-details-dialog.component'; // Import the new dialog component

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule, // Keep MatDialogModule
    NgApexchartsModule // Add NgApexchartsModule
  ],
  templateUrl: './company-list.component.html',
  styleUrl: './company-list.component.css'
})
export class CompanyListComponent implements OnInit, AfterViewInit {
  displayedColumns: string[] = ['compCd', 'compNm', 'athoCap', 'paidCap', 'regOff', 'noShrs', 'instrCd', 'startDt', 'fcVal', 'actions'];
  dataSource = new MatTableDataSource<any>();
  companies: any[] = []; // Keep this for now, might be removed later
  search: string = '';
  // selectedCompanyCompCd: number | null = null; // Remove this

  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private companyService: CompanyService, private dialog: MatDialog) { } // Inject MatDialog

  ngOnInit(): void {
    // Initial load is handled in ngAfterViewInit
  }

  ngAfterViewInit(): void {
    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.companyService.getCompanies(
            this.search,
            this.paginator.pageIndex + 1, // MatPaginator is 0-based, API is 1-based
            this.paginator.pageSize,
            this.sort.active,
            this.sort.direction
          ).pipe(catchError(() => observableOf(null)));
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = data === null;

          if (data === null) {
            return [];
          }

          // Only refresh the result length if there is new data. In case of rate
          // limit errors, we do not want to reset the paginator to zero, as that
          // would prevent users from re-triggering requests.
          this.resultsLength = data.totalCount;
          return data.companies;
        }),
      )
      .subscribe(data => (this.dataSource.data = data));
  }

  onSearch(): void {
    this.paginator.pageIndex = 0; // Reset to first page on new search
    this.loadCompanies(); // Trigger data load
  }

  loadCompanies(): void {
    // This method is now primarily used to trigger a data refresh
    // by changing paginator or sort, which then triggers the merge pipe.
    // For search, we manually trigger it by resetting pageIndex.
    // If you need to manually trigger a refresh without changing paginator/sort,
    // you might need a Subject or similar.
    this.paginator.page.emit(); // Emit a page event to trigger data load
  }

  openCompanyDetails(compCd: number): void {
    this.dialog.open(CompanyDetailsDialogComponent, {
      width: '800px', // Adjust width as needed
      data: { compCd: compCd }
    });
  }

  // closeCompanyDetails(): void { // Remove this
  //   this.selectedCompanyCompCd = null;
  // }
}