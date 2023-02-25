import { FileService } from 'src/app/_services/file.service';
import { AdminCategoryComponent } from './../admin-category/admin-category.component';
import { IdArray } from 'src/app/_models/adminRequest';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CategoryService } from 'src/app/_services/category.service';
import { ManagerCategoryDetail } from 'src/app/_models/category';
import { fnGetObjectStateString, fnGetObjectStateStyle } from 'src/app/_common/function/style-class';

@Component({
  selector: 'app-admin-category-detail',
  templateUrl: './admin-category-detail.component.html',
  styleUrls: ['./admin-category-detail.component.css']
})
export class AdminCategoryDetailComponent implements OnInit {
  id: IdArray;
  category: ManagerCategoryDetail;

  @ViewChild('categoryList') categoryList: AdminCategoryComponent;

  constructor(private categoryService: CategoryService, private route: ActivatedRoute, private router: Router, private toastr: ToastrService, private fileService: FileService) { }

  ngOnInit(): void {
    const categoryId = this.route.snapshot.paramMap.get('id');
    this.id = {
      ids: [+categoryId]
    };
    this.loadCategoryDetail(+categoryId);
  }

  loadCategoryDetail(categoryId: number) {
    this.categoryService.getManagerCategory(categoryId).subscribe(result => {
      this.category = result;
      setTimeout(() => {
        this.categoryList.filterByParentId(this.category.id);
      }, 200);
    })
  }

  getCategoryState() {
    return fnGetObjectStateString(this.category.status);
  }

  getCategoryStateStyle() {
    return fnGetObjectStateStyle(this.category.status);
  }

  editCategory() {
    this.router.navigateByUrl(
      '/administrator/category-manager/edit/' + this.id.ids[0]
    );
  }

  hideCategory() {    
    this.categoryService.hideCategories(this.id).subscribe((result) => {
      this.loadCategoryDetail(this.id.ids[0]);
      this.toastr.success('Category have been hidden or unhidden', 'Success');
    }, 
    error => 
    {
      this.toastr.error("Something wrong happen!", 'Error');
    });
  }

  deleteCategory() {
    this.categoryService.deleteCategories(this.id.ids).subscribe((result) => {
      this.loadCategoryDetail(this.id.ids[0]);
      this.toastr.success('Category have been deleted', 'Success');
    }, 
    error => 
    {
      this.toastr.error("Something wrong happen!", 'Error');
    });
  }

}
