import { ManagerCarousel } from 'src/app/_models/carousel';
import { Component, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-carousel-preview',
  templateUrl: './carousel-preview.component.html',
  styleUrls: ['./carousel-preview.component.css'],
})
export class CarouselPreviewComponent implements OnInit {
  @Input() carousel: ManagerCarousel;
  carouselTextStyle: string;
  constructor() {}

  ngOnInit(): void {
    this.carouselTextStyle = '';
    this.setCarouselStyle(this.carousel);
    // this.setCarouselPadding(this.carousel);
    // this.setPreviewImage(this.carousel);
  }

  setCarouselStyle(carousel: ManagerCarousel) {
    this.carouselTextStyle = '';
    this.setPositionStyle(carousel);
    this.setCarouselPadding(carousel);
    this.setPreviewImage(carousel);
  }

  setPositionStyle(carousel: ManagerCarousel) {
    switch (carousel.textPosition) {
      // Top left
      case 0:
        this.carouselTextStyle = 'top: 2vw; left: 2vw;';
        break;
      // Top center
      case 1:
        this.carouselTextStyle = 'top: 2vw; margin-left: auto; margin-right: auto; left: 0; right: 0; text-align: center;';
        break;
      // Top right
      case 2:
        this.carouselTextStyle = 'top: 2vw; right: 2vw; text-align: right;';
        break;
      // Middle left
      case 3:
        this.carouselTextStyle = 'top: 50%; transform: translateY(-50%); left: 2vw;';
        break;
      // Middle center
      case 4:
        this.carouselTextStyle = 'top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center;';
        break;
      // Middle right
      case 5:
        this.carouselTextStyle = 'top: 50%; transform: translateY(-50%); right: 2vw; text-align: right;';
        break;
      // Bottom left
      case 6:
        this.carouselTextStyle = 'bottom: 2vw; left: 2vw;';
        break;
      // Bottom center
      case 7:
        this.carouselTextStyle = 'bottom : 2vw; margin-left: auto; margin-right: auto; left: 0; right: 0; text-align: center;';
        break;
      // Bottom right
      case 8:
        this.carouselTextStyle = 'bottom: 2vw; right: 2vw;text-align: right;';
        break;
      default:
        break;
    }
  }

  setCarouselPadding(carousel: ManagerCarousel) {
    this.carouselTextStyle +=
      'padding: ' +
      carousel.textPaddingTop +
      'px' +
      ' ' +
      carousel.textPaddingRight +
      'px' +
      ' ' +
      carousel.textPaddingBottom +
      'px' +
      ' ' +
      carousel.textPaddingLeft +
      'px' +
      ';';

  }

  setPreviewImage(carousel: ManagerCarousel)
  {
    this.carousel.imageUrl = carousel.imageUrl;
  }
}
