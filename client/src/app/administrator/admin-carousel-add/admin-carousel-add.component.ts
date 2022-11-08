import { FileUploader } from 'ng2-file-upload';
import { ContentService } from 'src/app/_services/content.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TextPosition, ManagerCarousel, AvailableTextPositions } from 'src/app/_models/carousel';
import { concatMap, debounceTime, min, switchMap, tap } from 'rxjs/operators';
import { CarouselPreviewComponent } from 'src/app/_common/carousel-preview/carousel-preview.component';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin-carousel-add',
  templateUrl: './admin-carousel-add.component.html',
  styleUrls: ['./admin-carousel-add.component.css'],
})
export class AdminCarouselAddComponent implements OnInit {
  @ViewChild(CarouselPreviewComponent) preview: CarouselPreviewComponent;
  newCarouselForm: FormGroup;
  validationErrors: string[] = [];

  fileUploader: FileUploader;
  file: File;
  fileUrl = environment.fileUrl;
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
    private router: Router,
    private authenticationService: AuthenticationService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.initializeUploader();
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
      textPosition: [this.previewCarousel.textPosition,
        [Validators.required, Validators.min(0), Validators.max(7)]],
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

  initializeUploader() {
    let uploadUrl = this.fileUrl + "/image?" + "width=1200" + "&height=600";
    this.fileUploader = new FileUploader({
      url: uploadUrl,
      authToken: 'Bearer ' + this.authenticationService.userValue.jwtToken,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 3 * 1024 * 1024,
    });

    this.fileUploader.onAfterAddingFile = (file) => {
      file.withCredentials = true;
    };

    this.fileUploader.onErrorItem = (item, response, status, headers) => {
    }

    this.fileUploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        this.newCarouselForm.controls['imageUrl'].setValue(
          response
        );
        this.newCarouselForm.controls['textPosition'].setValue(
          +(this.previewCarousel.textPosition)
        );
        this.contentService.addCarousel(this.newCarouselForm.value).subscribe(
          (response) => {
            this.isUploadingFile = false;
            if (this.selectedButton.submitter.name == 'saveAndContinue')
            {
              this.setDefaultPreview();
              this.initializeForm();
              this.preview.setCarouselStyle(this.previewCarousel);

              this.router.navigateByUrl('/administrator/carousel-manager/add');

            }
            else this.router.navigateByUrl('/administrator/carousel-manager');
          },
          (error) => {
            this.validationErrors = error;
          }
        );

      };
    }
  }

  addNewCarousel(event) {
    this.selectedButton = event;
    this.isUploadingFile = true;
    this.fileUploader.uploadAll();
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
      textPosition: +(this.newCarouselForm.controls['textPosition'].value),
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

  setDefaultPreview()
  {
    this.previewCarousel = this.defaultPreviewCarousel;
    this.previewCarousel.imageUrl = '';
  }
}
