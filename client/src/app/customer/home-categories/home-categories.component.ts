import { CategoryService } from 'src/app/_services/category.service';
import { Component, OnInit } from '@angular/core';
import { Category, fnGetGenderName } from 'src/app/_models/category';

@Component({
  selector: 'app-home-categories',
  templateUrl: './home-categories.component.html',
  styleUrls: ['./home-categories.component.css']
})
export class HomeCategoriesComponent implements OnInit {
  promotedCategories: Category[] = [];
  constructor(private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadPromotedCategories();
  }

  loadPromotedCategories()
  {
    this.categoryService.getPromotedCategories().subscribe(result => 
      {
        this.promotedCategories = result;
      }

    )
  }

  getGenderString(gender: number)
  {
    return fnGetGenderName(gender);
  }
}
