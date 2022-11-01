import { Component, OnInit } from '@angular/core';
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

  constructor(private authenticationService: AuthenticationService, private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadCategoryGroup();
  }

  navigationBarToggle() {
    this.collapseNavbar = !this.collapseNavbar;
    console.log(this.collapseNavbar);
  }

  searchBarToggle() {
    this.collapseSearchbar = !this.collapseSearchbar;
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

  categoryWindowToggle() {
    this.collapseCategory = !this.collapseCategory;
  }

  setCollapseCategory(value: boolean)
  {
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

}
