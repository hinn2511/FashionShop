import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { slide } from 'src/app/_common/animation/carousel.animations';
import { Carousel } from 'src/app/_models/carousel';

@Component({
  selector: 'app-product-images',
  templateUrl: './product-images.component.html',
  styleUrls: ['./product-images.component.css'],
  animations: [ slide]
})
export class ProductImagesComponent implements OnInit {
  @Input() carousels: Carousel[] = [];
  @Input() nextSlideInterval = 10000;
  currentSlideIndex = 0;
  currentUrl: string = "";
  startPos: string = "-100";
  endPos: string = "-100";
  currentSlide: Carousel;
  action: string = "next";

  interval;
  carouselTextStyle: string;

  constructor() {
    //Not implement
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.carousels.length > 0)
      this.viewSlide(0);
  }

  ngOnInit() {
    this.preloadImages();
    this.currentUrl = this.carousels[0].imageUrl;
    this.currentSlide = this.carousels[0];
  }

  ngOnDestroy(): void {
  }

  onPreviousClick() {
    this.startPos = "-100";
    this.endPos = "-100";
    this.action = "prev"
    this.currentUrl = this.currentSlide.imageUrl;
    this.currentSlideIndex = this.currentSlideIndex - 1;

    if(this.currentSlideIndex < 0)
      this.currentSlideIndex = this.carousels.length - 1;

    this.currentSlide = this.carousels[this.currentSlideIndex];
  }

  onNextClick() {
    this.startPos = "100";
    this.endPos = "100";
    this.action = "next"
    this.currentUrl = this.currentSlide.imageUrl;
    this.currentSlideIndex = this.currentSlideIndex + 1;

    if(this.currentSlideIndex === this.carousels.length)
      this.currentSlideIndex = 0;

    this.currentSlide = this.carousels[this.currentSlideIndex];
  }

  viewSlide(index: number)
  {    
    if (this.currentSlide != undefined)
      this.currentUrl = this.currentSlide.imageUrl;
    
    let step = index - this.currentSlideIndex;
    
    if (this.currentSlideIndex < index)
    {
      this.action = "next";
      for(let i = 0; i < step; i++)
      {
        this.onNextClick();
      }
    }
    else
    {
      this.action = "prev";
      for(let i = 0; i < -step; i++)
      {
        this.onPreviousClick();
      }
    }
  }



  preloadImages() {
    for (const slide of this.carousels) {
      new Image().src = slide.imageUrl;
    }
  }
}
