import { FileService } from './../../_services/file.service';
import { FileUploader } from 'ng2-file-upload';
import { ContentService } from 'src/app/_services/content.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  TextPosition,
  ManagerCarousel,
  AvailableTextPositions,
} from 'src/app/_models/carousel';
import {
  concatMap,
  debounceTime,
  mergeMap,
  min,
  switchMap,
  tap,
} from 'rxjs/operators';
import { CarouselPreviewComponent } from 'src/app/_common/carousel-preview/carousel-preview.component';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { environment } from 'src/environments/environment';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-carousel-add',
  templateUrl: './admin-carousel-add.component.html',
  styleUrls: ['./admin-carousel-add.component.css'],
})
export class AdminCarouselAddComponent implements OnInit {
  @ViewChild(CarouselPreviewComponent) preview: CarouselPreviewComponent;
  newCarouselForm: FormGroup;
  validationErrors: string[] = [];

  image: File;
  isUploadingFile = false;

  previewCarousel: ManagerCarousel;
  defaultPreviewCarousel: ManagerCarousel = {
    id: -1,
    status: -1,
    imageUrl: '',
    title: 'Preview Carousel Title',
    description: 'With description at middle center position.',
    navigationText: 'Navigation link here!',
    link: '/preview-carousel',
    textPosition: 4,
    textPaddingLeft: 0,
    textPaddingRight: 0,
    textPaddingTop: 0,
    textPaddingBottom: 0,
  };

  availableTextPositions = AvailableTextPositions;

  selectedButton: any;

  constructor(
    private fb: FormBuilder,
    private contentService: ContentService,
    private fileService: FileService,
    private router: Router,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.setDefaultPreview();
    this.newCarouselForm = this.fb.group({
      title: [this.previewCarousel.title, Validators.required],
      imageUrl: [this.previewCarousel.imageUrl],
      link: [this.previewCarousel.link, Validators.required],
      navigationText: [
        this.previewCarousel.navigationText,
        Validators.required,
      ],
      description: [this.previewCarousel.description],
      textPosition: [
        this.previewCarousel.textPosition,
        [Validators.required, Validators.min(0), Validators.max(7)],
      ],
      textPaddingTop: [
        this.previewCarousel.textPaddingTop,
        [Validators.required, Validators.min(0), Validators.max(999)],
      ],
      textPaddingBottom: [
        this.previewCarousel.textPaddingBottom,
        [Validators.required, Validators.min(0), Validators.max(999)],
      ],
      textPaddingLeft: [
        this.previewCarousel.textPaddingLeft,
        [Validators.required, Validators.min(0), Validators.max(999)],
      ],
      textPaddingRight: [
        this.previewCarousel.textPaddingRight,
        [Validators.required, Validators.min(0), Validators.max(999)],
      ],
    });
    this.newCarouselForm.valueChanges.subscribe(() => this.reload());
  }

 

  addNewCarousel(event) {
    this.selectedButton = event;
    this.isUploadingFile = true;

    this.fileService
      .uploadImage(this.image, 1500, 600, this.authenticationService.userValue.jwtToken)
      .pipe(
        concatMap((uploadResult) =>
          this.contentService.addCarousel(
            uploadResult.url,
            this.previewCarousel
          )
        )
      )
      .subscribe((result) => {
        this.isUploadingFile = false;
        this.toastr.success('Carousel have been added', 'Success');
        if (this.selectedButton.submitter.name == 'saveAndContinue') {
          this.setDefaultPreview();
          this.initializeForm();
          this.preview.setCarouselStyle(this.previewCarousel);

          this.router.navigateByUrl('/administrator/carousel-manager/add');
        } else this.router.navigateByUrl('/administrator/carousel-manager');
      }, 
      error => 
      {
        this.toastr.error("Something wrong happen!", 'Error');
      });
  }

  reload() {
    this.previewCarousel = {
      id: -1,
      status: -1,
      imageUrl: this.newCarouselForm.controls['imageUrl'].value,
      title: this.newCarouselForm.controls['title'].value,
      description: this.newCarouselForm.controls['description'].value,
      navigationText: this.newCarouselForm.controls['navigationText'].value,
      link: this.newCarouselForm.controls['link'].value,
      textPosition: +this.newCarouselForm.controls['textPosition'].value,
      textPaddingLeft: this.newCarouselForm.controls['textPaddingLeft'].value,
      textPaddingRight: this.newCarouselForm.controls['textPaddingRight'].value,
      textPaddingTop: this.newCarouselForm.controls['textPaddingTop'].value,
      textPaddingBottom:
        this.newCarouselForm.controls['textPaddingBottom'].value,
    };
    this.preview.setCarouselStyle(this.previewCarousel);
  }

  previewImage(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.image = event.target.files[0];
      var reader = new FileReader();
      reader.onload = (event: any) => {
        this.previewCarousel.imageUrl = event.target.result;
        this.newCarouselForm.controls['imageUrl'].setValue(
          this.previewCarousel.imageUrl
        );
        this.preview.setCarouselStyle(this.previewCarousel);
      };
      reader.readAsDataURL(event.target.files[0]);
    }
  }

  setDefaultPreview() {
    this.previewCarousel = this.defaultPreviewCarousel;
    this.previewCarousel.imageUrl = '';
  }
}
