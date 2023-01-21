import { ArticleService } from 'src/app/_services/article.service';
import { FileService } from './../../_services/file.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ArticleType, ArticleTypeList } from 'src/app/_models/article';
import { Router } from '@angular/router';
import { AddArticle } from 'src/app/_models/article';
import { concatMap } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { ToastrService } from 'ngx-toastr';
import {
  fnGetFormControlValue,
  fnUpdateFormControlStringValue,
} from 'src/app/_common/function/function';
import { FileUploadedResponse } from 'src/app/_models/file';

@Component({
  selector: 'app-admin-article-add',
  templateUrl: './admin-article-add.component.html',
  styleUrls: ['./admin-article-add.component.css'],
})
export class AdminArticleAddComponent implements OnInit {
  newArticleForm: FormGroup;
  image: File;
  imagePreviewUrl: string = '';
  isUploadingFile = false;
  articleTypes: ArticleType[] = ArticleTypeList;
  selectedButton: any;

  constructor(
    private fb: FormBuilder,
    private articleService: ArticleService,
    private fileService: FileService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.newArticleForm = this.fb.group({
      headline: ['', Validators.required],
      foreword: [''],
      thumbnailUrl: ['', Validators.required],
      content: ['', Validators.required],
      contentType: [0, Validators.required],
    });
  }

  addNewArticle(event) {
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
          let newArticle: AddArticle = this.convertToArticle(uploadResult);
          return this.articleService.addArticle(newArticle);
        })
      )
      .subscribe(
        (result) => {
          this.isUploadingFile = false;
          this.toastr.success('Article have been added', 'Success');
          if (this.selectedButton.submitter.name == 'saveAndContinue') {
            this.initializeForm();
            this.router.navigateByUrl('/administrator/article-manager/add');
          } else this.router.navigateByUrl('/administrator/article-manager');
        },
        (error) => {
          this.toastr.error('Something wrong happen!', 'Error');
        }
      );
  }

  private convertToArticle(uploadResult: FileUploadedResponse): AddArticle {
    return {
      headline: fnGetFormControlValue(this.newArticleForm, 'headline'),
      foreword: fnGetFormControlValue(this.newArticleForm, 'foreword'),
      thumbnailUrl: uploadResult.url,
      content: fnGetFormControlValue(this.newArticleForm, 'content'),
      contentType: +fnGetFormControlValue(this.newArticleForm, 'contentType'),
    };
  }

  previewImage(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.image = event.target.files[0];
      const reader = new FileReader();
      reader.onload = (event: any) => {
        this.imagePreviewUrl = event.target.result;
        fnUpdateFormControlStringValue(
          this.newArticleForm,
          'thumbnailUrl',
          'url',
          false
        );
      };
      reader.readAsDataURL(event.target.files[0]);
    }
  }

  updateContent($event: string) {
    fnUpdateFormControlStringValue(
      this.newArticleForm,
      'content',
      $event,
      false
    );
  }

}
