import { Router } from '@angular/router';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.css'],
})
export class SearchBarComponent implements OnInit {

  searchQuery: string ;
  icon: string;
  action: string = 'search';
  @ViewChild("search") searchTextBox : ElementRef;

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Not implement
    setTimeout(() => {
      this.searchTextBox.nativeElement.focus();      
    }, 200);
  }

  updateAction() {    
    if (this.searchQuery != "") this.action = 'clear';
    else this.action = 'search';
  }

  clearSearchQuery() {
    this.searchQuery = "";
  }

  showSearchResult() {
    if (this.searchQuery == "" || this.searchQuery == null)
      return;
    this.router.navigate(['/search'], {
      queryParams: { q: this.searchQuery },
    });
  }
}
