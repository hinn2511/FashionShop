import { concatMap } from 'rxjs/operators';
import { DialogService } from 'src/app/_services/dialog.service';
import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { RotateAnimation } from 'src/app/_common/animation/carousel.animations';
import { IdArray } from 'src/app/_models/adminRequest';
import {
  Gender,
  GenderList,
  ManagerCategory,
  ManagerCategoryParams,
} from 'src/app/_models/category';
import { GenericStatus, GenericStatusList } from 'src/app/_models/generic';
import { Pagination } from 'src/app/_models/pagination';
import { CategoryService } from 'src/app/_services/category.service';
import { fnGetObjectStateString, fnGetObjectStateStyle } from 'src/app/_common/function/style-class';

@Component({
  selector: 'app-admin-category',
  templateUrl: './admin-category.component.html',
  styleUrls: ['./admin-category.component.css'],
  animations: [RotateAnimation],
})
export class AdminCategoryComponent implements OnInit {

  @Input() minimize: boolean = false;
  categories: ManagerCategory[];
  pagination: Pagination;
  categoryParams: ManagerCategoryParams;
  isSelectAllCategory: boolean;
  state: string = 'default';
  showStatusFilter: boolean;
  showGenderFilter: boolean;
  selectedIds: number[] = [];
  genericStatus: GenericStatus[] = GenericStatusList;

  genders: Gender[] = GenderList;
  query: string;

  constructor(
    private categoryService: CategoryService,
    private router: Router,
    private toastr: ToastrService,
    private dialogService: DialogService
  ) {
    this.categoryParams = this.categoryService.getManagerCategoryParams();
  }

  ngOnInit(): void {
    this.categoryParams.field = 'Id';
    this.categoryParams.orderBy = 0;
    this.categoryParams.parentId = 0;
    this.categoryParams.categoryStatus = [0, 1];
    this.categoryParams.genders = [0];
    this.isSelectAllCategory = false;
    this.showStatusFilter = false;
    this.loadCategories();
  }

  rotate() {
    this.state = this.state === 'default' ? 'rotated' : 'default';
  }

  loadCategories() {    
    this.categoryService.setManagerCategoryParams(this.categoryParams);

    this.categoryService
      .getManagerCategories(this.categoryParams)
      .subscribe((response) => {
        this.categories = response.result;

        this.pagination = response.pagination;
      });
  }

  resetSelectedIds() {
    this.selectedIds = [];
  }

  
  filterByParentId(parentId: number) {
    this.categoryParams.parentId = parentId;
    this.loadCategories();
  }

  viewDetail(categoryId: number) {
    this.router.navigateByUrl(
      '/administrator/category-manager/detail/' + categoryId
    );
  }

  editCategory() {
    if (!this.isSingleSelected()) return;
    this.router.navigateByUrl(
      '/administrator/category-manager/edit/' + this.selectedIds[0]
    );
  }

  hideCategories() {
    // if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.categoryService.hideCategories(ids).subscribe(
      (result) => {
        this.loadCategories();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  activateCategories() {
    // if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.categoryService.activateCategories(ids).subscribe(
      (result) => {
        this.loadCategories();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  setEditorChoice() {
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.categoryService.promoteCategories(ids).subscribe(
      (result) => {
        this.loadCategories();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error(error.message, 'Error');
      }
    );
  }

  removeEditorChoice() {
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.categoryService.demoteCategories(ids).subscribe(
      (result) => {
        this.loadCategories();
        this.resetSelectedIds();
        this.toastr.success(result.message, 'Success');
      },
      (error) => {
        this.toastr.error(error.message, 'Error');
      }
    );
  }

  deleteCategories() {
    if (!this.isMultipleSelected()) return;

    let accept = false;

    this.dialogService
      .openConfirmDialog(
        'Confirm',
        `Are you sure you want to delete ${this.selectedIds.length} categories and all sub categories? This action can not be undone!.`,
        false,
        'Yes',
        'No'
      )
      .pipe(
        concatMap((result) => {
          if (result.result) {
            accept = true;
            return this.categoryService.deleteCategories(this.selectedIds);
          }
        })
      )
      .subscribe(
        (result) => {
          if (accept) {
            this.loadCategories();
            this.resetSelectedIds();
            this.toastr.success(result.message, 'Success');
          }
        },
        (error) => {
          this.toastr.error(error.message, 'Error');
        }
      );
  }

  pageChanged(event: any) {
    if (this.categoryParams.pageNumber !== event.page) {
      this.categoryParams.pageNumber = event.page;
      this.categoryService.setManagerCategoryParams(this.categoryParams);
      this.loadCategories();
    }
  }

  sort(type: number) {
    this.categoryParams.orderBy = type;
    this.loadCategories();
  }

  filter(params: ManagerCategoryParams) {
    this.categoryParams = params;
    this.loadCategories();
  }

  resetFilter() {
    this.categoryParams = this.categoryService.resetManagerCategoryParams();
    this.loadCategories();
  }

  orderBy(field: string) {
    switch (field) {
      case 'id':
        this.categoryParams.field = 'Id';
        break;
      case 'categoryName':
        this.categoryParams.field = 'CategoryName';
        break;
      case 'gender':
        this.categoryParams.field = 'Gender';
        break;
      case 'status':
        this.categoryParams.field = 'Status';
        break;
      case 'promoted':
        this.categoryParams.field = 'Promoted';
        break;
      case 'parentCategory':
        this.categoryParams.field = 'ParentCategory';
        break;
      default:
        this.categoryParams.field = 'DateCreated';
        break;
    }
    if (this.categoryParams.orderBy == 0) this.categoryParams.orderBy = 1;
    else this.categoryParams.orderBy = 0;
    this.rotate();
    this.loadCategories();
  }

  selectAllCategories() {
    if (this.isSelectAllCategory) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.categories.map(({ id }) => id);
    }
    this.isSelectAllCategory = !this.isSelectAllCategory;
  }

  selectCategory(id: number) {
    if (this.selectedIds.includes(id)) {
      this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
    } else {
      this.selectedIds.push(id);
    }
  }

  isCategorySelected(id: number) {
    return this.selectedIds.indexOf(id) >= 0;
  }

  getCategoryState(category: ManagerCategory) {
    return fnGetObjectStateString(category.status);
  }

  getStateStyle(category: ManagerCategory) {
    return fnGetObjectStateStyle(category.status);
  }

  isAllStatusIncluded() {
    return (
      this.categoryParams.categoryStatus.length == this.genericStatus.length
    );
  }

  isStatusIncluded(status: number) {
    return this.categoryParams.categoryStatus.indexOf(status) > -1;
  }

  selectStatus(status: number) {
    if (this.isStatusIncluded(status))
      this.categoryParams.categoryStatus =
        this.categoryParams.categoryStatus.filter((x) => x !== status);
    else this.categoryParams.categoryStatus.push(status);
    this.categoryParams.categoryStatus = [
      ...this.categoryParams.categoryStatus,
    ].sort((a, b) => a - b);
    this.loadCategories();
  }

  selectAllCategoryStatus() {
    if (this.isAllStatusIncluded()) this.categoryParams.categoryStatus = [];
    else
      this.categoryParams.categoryStatus = this.genericStatus.map((x) => x.id);
    this.loadCategories();
  }

  isAllGenderIncluded() {
    return this.categoryParams.genders.length == this.genders.length;
  }

  isGenderIncluded(status: number) {
    return this.categoryParams.genders.indexOf(status) > -1;
  }

  selectGender(status: number) {
    if (this.isGenderIncluded(status))
      this.categoryParams.genders = this.categoryParams.genders.filter(
        (x) => x !== status
      );
    else this.categoryParams.genders.push(status);
    this.categoryParams.genders = [...this.categoryParams.genders].sort(
      (a, b) => a - b
    );
    this.loadCategories();
  }

  selectAllGender() {
    if (this.isAllGenderIncluded()) this.categoryParams.genders = [];
    else this.categoryParams.genders = this.genders.map((x) => x.id);
    this.loadCategories();
  }

  genderFilterToggle() {
    this.showGenderFilter = !this.showGenderFilter;
  }

  statusFilterToggle() {
    this.showStatusFilter = !this.showStatusFilter;
  }

  isSingleSelected() {
    return this.selectedIds.length == 1;
  }

  isMultipleSelected() {
    return this.selectedIds.length >= 1;
  }
}
