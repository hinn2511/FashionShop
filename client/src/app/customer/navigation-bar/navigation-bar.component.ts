import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Catalogue, CategoryCatalogue, SubCategoryCatalogue } from 'src/app/_models/category';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { CategoryService } from 'src/app/_services/category.service';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {
  collapseNavbar: boolean = true;
  collapseSearchbar: boolean = true;
  collapseCategory: boolean = true;
  categoryGroups: Catalogue[]= [];
  selectedCategoryGroup: Catalogue;
  categories: CategoryCatalogue[]= [];
  selectedCategory: CategoryCatalogue;
  selectedSubCategory: SubCategoryCatalogue;
  categoryWindowMargin: string;
  
  constructor(private authenticationService: AuthenticationService, private categoryService: CategoryService, private router: Router) { }
  
  ngOnInit(): void {
    this.loadCategoryGroup();
  }
  
  navigationBarToggle() {
    let state = this.collapseNavbar;
    this.collapseAll();
    this.collapseNavbar = !state;
  }
  searchBarToggle() {    
    let state = this.collapseSearchbar;
    this.collapseAll();
    this.collapseSearchbar = !state;
    
  }
  
  categoryWindowToggle() {
    let state = this.collapseCategory;
    this.collapseAll();
    this.collapseCategory = !state;
  }

  collapseAll()
  {
    this.collapseCategory = true;
    this.collapseNavbar = true;
    this.collapseSearchbar = true;
  }

  logout() {
    this.authenticationService.logout();
  }

  isUserExist(): boolean {
    return this.authenticationService.userValue !== null && this.authenticationService.userValue !== undefined;
  }

  loadCategoryGroup()
  {
    this.categoryService.getCatalogue().subscribe(result =>
      {
        this.categoryGroups = result;
      });
  }


  setCollapseCategory(value: boolean)
  {
    if(value)
      this.collapseAll();
    this.collapseCategory = value;
  }

  selectCategoryGroup(index: number)
  {
    this.selectedCategoryGroup = this.categoryGroups[index];
  }

  isCategoryGroupSelected(gender: number)
  {
    if(this.selectedCategoryGroup == undefined || this.selectedCategoryGroup == null)
      return false;
    return this.selectedCategoryGroup.gender == gender;
  }

  selectCategory(index: number)
  {
    this.selectedCategory = this.selectedCategoryGroup.categories[index];
  }

  viewCategory(categoryName: string, categorySlug: string, gender: number)
  {
    this.categoryService.setCurrentCategory(categoryName, gender);
    this.router.navigate(
      ['/products'],
      { queryParams: { category: categorySlug, 'gender': gender } }
    );
  }

}
