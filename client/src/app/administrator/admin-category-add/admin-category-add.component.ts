import { DialogService } from './../../_services/dialog.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { concatMap } from 'rxjs/operators';
import {
  fnGetFormControlValue,
  fnUpdateFormControlNumberValue,
  fnUpdateFormControlStringValue,
} from 'src/app/_common/function/function';
import {
  AddCategory,
  Gender,
  GenderList,
  ManagerCatalogue,
} from 'src/app/_models/category';
import { FileUploadedResponse } from 'src/app/_models/file';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { CategoryService } from 'src/app/_services/category.service';
import { FileService } from 'src/app/_services/file.service';

@Component({
  selector: 'app-admin-category-add',
  templateUrl: './admin-category-add.component.html',
  styleUrls: ['./admin-category-add.component.css'],
})
export class AdminCategoryAddComponent implements OnInit {
  newCategoryForm: FormGroup;
  image: File;
  imagePreviewUrl: string = '';
  isUploadingFile = false;
  genders: Gender[] = GenderList;
  selectedButton: any;
  catalogue: ManagerCatalogue[] = [];
  selectedParentCategoryName: string = 'None';

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private fileService: FileService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private dialogService: DialogService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadCatalogue();
  }

  initializeForm() {
    this.newCategoryForm = this.fb.group({
      categoryName: ['', Validators.required],
      categoryImageUrl: ['', Validators.required],
      parentId: [0],
      gender: [0, Validators.required],
    });
  }

  addNewCategory(event) {
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
          let newCategory: AddCategory = this.convertToCategory(uploadResult);
          return this.categoryService.addCategory(newCategory);
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

  private convertToCategory(uploadResult: FileUploadedResponse): AddCategory {
    return {
      categoryName: fnGetFormControlValue(this.newCategoryForm, 'categoryName'),
      categoryImageUrl: uploadResult.url,
      parentId: fnGetFormControlValue(this.newCategoryForm, 'parentId'),
      gender: +fnGetFormControlValue(this.newCategoryForm, 'gender'),
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
        GenderList.filter(
          (x) => x.id == fnGetFormControlValue(this.newCategoryForm, 'gender')
        ).map((g) => g.name),
        'Select parent category',
        'Finish',
        'Cancel'
      )
      .subscribe((selectedResult) => {
        if (selectedResult.result) {
          this.selectedParentCategoryName = selectedResult.selectedValue;
          fnUpdateFormControlNumberValue(
            this.newCategoryForm,
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
          this.newCategoryForm,
          'categoryImageUrl',
          'url',
          false
        );
      };
      reader.readAsDataURL(event.target.files[0]);
    }
  }
}
