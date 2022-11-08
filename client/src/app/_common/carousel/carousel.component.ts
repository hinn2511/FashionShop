import { CustomerCarousel } from './../../_models/carousel';
import { transition, trigger, useAnimation } from '@angular/animations';
import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { interval, timer } from 'rxjs';
import { AnimationType, fadeIn, fadeOut, flipIn, flipOut, jackIn, jackOut, scaleIn, scaleOut } from '../animation/carousel.animations';

@Component({
  selector: 'app-carousel',
  templateUrl: './carousel.component.html',
  styleUrls: ['./carousel.component.css'],
  animations: [
    trigger("slideAnimation", [
      /* scale */
      transition("void => scale", [
        useAnimation(scaleIn, { params: { time: "500ms" } })
      ]),
      transition("scale => void", [
        useAnimation(scaleOut, { params: { time: "500ms" } })
      ]),

      /* fade */
      transition("void => fade", [
        useAnimation(fadeIn, { params: { time: "500ms" } })
      ]),
      transition("fade => void", [
        useAnimation(fadeOut, { params: { time: "500ms" } })
      ]),

      /* flip */
      transition("void => flip", [
        useAnimation(flipIn, { params: { time: "500ms" } })
      ]),
      transition("flip => void", [
        useAnimation(flipOut, { params: { time: "500ms" } })
      ]),

      /* JackInTheBox */
      transition("void => jackInTheBox", [
        useAnimation(jackIn, { params: { time: "700ms" } })
      ]),
      transition("jackInTheBox => void", [
        useAnimation(jackOut, { params: { time: "700ms" } })
      ])
    ])
  ]
})
export class CarouselComponent implements OnChanges, OnInit, OnDestroy {
  @Input() carousels: CustomerCarousel[] = [];
  @Input() animationType = AnimationType.Fade;
  @Input() nextSlideInterval = 10000;
  currentSlideIndex = 0;
  currentSlide: CustomerCarousel;

  interval;
  carouselTextStyle: string;

  constructor() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.carousels.length > 0)
      this.viewSlide(0);
  }

  ngOnInit() {
    this.preloadImages();
    this.interval = setInterval(() => {
      this.onNextClick()
    },  this.nextSlideInterval);
  }

  ngOnDestroy(): void {
    clearInterval(this.interval);
  }

  onPreviousClick() {
    this.currentSlideIndex = this.currentSlideIndex - 1;

    if(this.currentSlideIndex < 0)
      this.currentSlideIndex = this.carousels.length - 1;

    this.currentSlide = this.carousels[ this.currentSlideIndex];
    this.setCarouselStyle();
    this.restartInterval();
  }

  onNextClick() {
    this.currentSlideIndex = this.currentSlideIndex + 1;

    if(this.currentSlideIndex === this.carousels.length)
      this.currentSlideIndex = 0;

    this.currentSlide = this.carousels[this.currentSlideIndex];
    this.setCarouselStyle();
    this.restartInterval();
  }

  viewSlide(index: number)
  {
    this.currentSlideIndex = index;
    this.currentSlide = this.carousels[this.currentSlideIndex];
    this.setCarouselStyle();
    this.restartInterval();
  }



  preloadImages() {
    for (const slide of this.carousels) {
      new Image().src = slide.imageUrl;
    }
  }

  restartInterval() {
    clearInterval(this.interval);
    this.interval = setInterval(() => {
      this.onNextClick();
    }, this.nextSlideInterval);
  }



  setCarouselStyle() {
    switch (this.currentSlide.textPosition) {
      case 0:
        this.carouselTextStyle = 'top: 2vw; left: 2vw;';
        break;
      case 1:
        this.carouselTextStyle = 'top: 2vw; left: 50%; text-align: center;';
        break;
      case 2:
        this.carouselTextStyle = 'top: 2vw; right: 2vw;';
        break;
      case 3:
        this.carouselTextStyle = 'top: 50%; left: 2vw;';
        break;
      case 4:
        this.carouselTextStyle = 'top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center;';
        break;
      case 5:
        this.carouselTextStyle = 'top: 50%; right: 2vw;';
        break;
      case 6:
        this.carouselTextStyle = 'bottom: 2vw; left: 2vw;';
        break;
      case 7:
        this.carouselTextStyle = 'bottom : 2vw; left: 50%; text-align: center;';
        break;
      default:
        this.carouselTextStyle = 'bottom: 2vw; right: 2vw;';
        break;
    }
    this.addPadding();
  }

  addPadding() {
    this.carouselTextStyle +=
      'padding: ' +
      this.currentSlide.textPaddingTop +
      'px' +
      ' ' +
      this.currentSlide.textPaddingRight +
      'px' +
      ' ' +
      this.currentSlide.textPaddingBottom +
      'px' +
      ' ' +
      this.currentSlide.textPaddingLeft +
      'px' +
      ';';
  }
}
