import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { fnGetFormControlValue, fnUpdateFormControlStringValue } from 'src/app/_common/function/function';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.css']
})
export class SearchBarComponent implements OnInit {
  searchForm: FormGroup;
  searchBarIcon: string;

  constructor(private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.searchBarIcon = "fa-search";
    this.searchForm = this.fb.group({
      query: [''],
    });
    this.searchForm.get('query').valueChanges.subscribe((val) => {
      let searchQuery = fnGetFormControlValue(this.searchForm, 'query');
        if (searchQuery.length > 0)
          this.searchBarIcon = "fa-times";
        else
          this.searchBarIcon = "fa-search";
    });
  }


  clearSearchQuery()
  {
    fnUpdateFormControlStringValue(this.searchForm, 'query', '', false);
  }

  viewSearchResult() {
    this.router.navigate(['/search'], {
      queryParams: { q: fnGetFormControlValue(this.searchForm, 'query') },
    });
  }
}
