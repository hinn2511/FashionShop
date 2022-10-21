import { AnimationType } from './../../_common/animation/carousel.animations';
import { calculatePreviewOffset } from 'src/app/_common/function/global';
import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { fadeIn, fadeOut, flipIn, flipOut, jackIn, jackOut, scaleIn, scaleOut, SlideInOutAnimation } from 'src/app/_common/animation/carousel.animations';
import { ProductPhoto } from 'src/app/_models/product';
import { trigger, transition, useAnimation } from '@angular/animations';

@Component({
  selector: 'app-product-images',
  templateUrl: './product-images.component.html',
  styleUrls: ['./product-images.component.css'],
  animations: [
    trigger("slideAnimation", [

      /* fade */
      transition("void => fade", [
        useAnimation(fadeIn, { params: { time: "500ms" } })
      ]),
      transition("fade => void", [
        useAnimation(fadeOut, { params: { time: "500ms" } })
      ]),



    ])
  ]
})
export class ProductImagesComponent implements OnInit {
  @Input() viewerItems: ProductPhoto[] = [];
  @Input() maxPreviewItem: number;
  @Input() nextItemInterval = 7000;
  animationType = AnimationType.Fade;
  previewItems: ProductPhoto[] = [];
  currentItem: ProductPhoto;
  currentIndex: number;
  leftOffset: number;
  rightOffset: number;
  animationState = 'in';

  constructor() { }


  ngOnInit(): void {
    this.setItem(0);
    let offset = calculatePreviewOffset(this.viewerItems.length, this.maxPreviewItem, 0);
    this.leftOffset = offset[0];
    this.rightOffset = offset[1];
    this.preparePreview();
    this.preloadImages();
  }


  preloadImages() {
    for (const slide of this.viewerItems) {
      new Image().src = slide.url;
    }
  }

  private setItem(id: number) {
    this.currentIndex = id;
    this.currentItem = this.viewerItems[id];
  }

  preparePreview() {
    this.previewItems = this.viewerItems.slice(
      this.leftOffset,
      this.rightOffset
    );

  }

  isCurrent(url: string) {
    return url == this.currentItem.url;
  }

  getPreviewItemWidth() {
    return 'width: calc(' + 100 / this.maxPreviewItem + '% - 1px);';
  }

  onThumbnailClick(item: ProductPhoto) {
    this.setItem(this.viewerItems.indexOf(item));
    this.toggle();
  }

  onPreviousClick() {
    if (this.currentIndex > 0) {
      this.setItem(--this.currentIndex);
      if (this.viewerItems.length > this.maxPreviewItem) {
        this.leftOffset--;
        this.rightOffset--;
      }
    }
    this.preparePreview();
    this.toggle();
  }

  onNextClick() {
    if (this.currentIndex < this.viewerItems.length - 1) {
      this.setItem(++this.currentIndex);
      if (this.viewerItems.length > this.maxPreviewItem) {
        this.leftOffset++;
        this.rightOffset++;
      }
    }
    this.preparePreview();
    this.toggle();
  }

  toggle() {
    this.animationState = this.animationState === 'out' ? 'in' : 'out';
  }
}
