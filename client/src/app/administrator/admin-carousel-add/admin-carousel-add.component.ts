import { FileService } from './../../_services/file.service';
import { ContentService } from 'src/app/_services/content.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {
  ManagerCarousel,  
} from 'src/app/_models/carousel';
import {
  concatMap,
} from 'rxjs/operators';
import { CarouselPreviewComponent } from 'src/app/_common/carousel-preview/carousel-preview.component';
import { AuthenticationService } from 'src/app/_services/authentication.service';
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
    link: '/preview-carousel'
  };


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
      description: [this.previewCarousel.description],
     
    });
    this.newCarouselForm.valueChanges.subscribe(() => this.reload());
  }

 

  addNewCarousel(event) {
    this.selectedButton = event;
    this.isUploadingFile = true;

    this.fileService
      .uploadImage(this.image, 1600, 900, this.authenticationService.userValue.jwtToken)
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
      link: this.newCarouselForm.controls['link'].value,
    };
    this.preview.setCarouselStyle(this.previewCarousel);
  }

  previewImage(event: any) {
    if (event.target.files && event.target.files[0]) {
      this.image = event.target.files[0];
      const reader = new FileReader();
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
