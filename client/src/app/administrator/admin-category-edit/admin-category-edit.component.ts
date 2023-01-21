import { DialogService } from './../../_services/dialog.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { concatMap } from 'rxjs/operators';
import {
  fnGetFormControlValue,
  fnUpdateFormControlNumberValue,
  fnUpdateFormControlStringValue,
} from 'src/app/_common/function/function';
import {
  Gender,
  GenderList,
  ManagerCatalogue,
  ManagerCategory,
  UpdateCategory,
} from 'src/app/_models/category';
import { FileUploadedResponse } from 'src/app/_models/file';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { CategoryService } from 'src/app/_services/category.service';
import { FileService } from 'src/app/_services/file.service';

@Component({
  selector: 'app-admin-category-edit',
  templateUrl: './admin-category-edit.component.html',
  styleUrls: ['./admin-category-edit.component.css'],
})
export class AdminCategoryEditComponent implements OnInit {
  category: ManagerCategory;
  editCategoryForm: FormGroup;
  image: File;
  imagePreviewUrl: string = '';
  isUploadingFile = false;
  genders: Gender[] = GenderList;
  selectedButton: any;
  catalogue: ManagerCatalogue[] = [];
  selectedParentCategoryName: string = '';

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private fileService: FileService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private dialogService: DialogService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const categoryId = this.route.snapshot.paramMap.get('id');
    this.loadCategory(+categoryId);
    this.loadCatalogue();
  }

  loadCategory(categoryId: number) {
    this.categoryService.getManagerCategory(categoryId).subscribe((result) => {
      this.category = result;
      this.initializeForm();
      this.imagePreviewUrl = this.category.categoryImageUrl;
      if (this.category.parent != undefined)
        this.selectedParentCategoryName = `${this.category.genderName} - ${this.category.categoryName}`;
    });
  }

  initializeForm() {
    this.editCategoryForm = this.fb.group({
      categoryName: [this.category.categoryName, Validators.required],
      categoryImageUrl: [this.category.categoryImageUrl, Validators.required],
      parentId: [this.category.parent ?? 0],
      gender: [this.category.gender, Validators.required],
    });
  }

  editNewCategory(event) {
    this.selectedButton = event;
    this.isUploadingFile = true;
    this.fileService
      .uploadImage(
        this.image,
        0,
        0,
        this.authenticationService.userValue.jwtToken
      )
      .pipe(
        concatMap((uploadResult) => {
          let editedCategory: UpdateCategory = this.convertToCategory(uploadResult);
          return this.categoryService.editCategory(this.category.id, editedCategory);
        })
      )
      .subscribe(
        (result) => {
          this.isUploadingFile = false;
          this.toastr.success('Category have been added', 'Success');
          if (this.selectedButton.submitter.name == 'saveAndContinue') {
            this.initializeForm();
            this.router.navigateByUrl('/administrator/category-manager/add');
          } else this.router.navigateByUrl('/administrator/category-manager');
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  private convertToCategory(
    uploadResult: FileUploadedResponse
  ): UpdateCategory {
    return {
      categoryName: fnGetFormControlValue(
        this.editCategoryForm,
        'categoryName'
      ),
      categoryImageUrl: uploadResult.url,
      parentId: fnGetFormControlValue(this.editCategoryForm, 'parentId'),
      gender: +fnGetFormControlValue(this.editCategoryForm, 'gender'),
    };
  }

  loadCatalogue() {
    this.categoryService.getManagerCatalogue().subscribe((result) => {
      this.catalogue = result;
    });
  }

  openCatalogue() {
    this.dialogService
      .openCategorySingleSelectorDialog(
        this.catalogue,
        true,
        false,
        [],
        'Select parent category',
        'Finish',
        'Cancel'
      )
      .subscribe((selectedResult) => {
        if (selectedResult.result) {
          this.selectedParentCategoryName = selectedResult.selectedValue;
          fnUpdateFormControlNumberValue(
            this.editCategoryForm,
            'parentId',
            selectedResult.selectedId,
            false
          );
        }
      });
  }

  previewImage(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.image = event.target.files[0];
      const reader = new FileReader();
      reader.onload = (event: any) => {
        this.imagePreviewUrl = event.target.result;
        fnUpdateFormControlStringValue(
          this.editCategoryForm,
          'categoryImageUrl',
          'url',
          false
        );
      };
      reader.readAsDataURL(event.target.files[0]);
    }
  }
}
