import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, Inject, Input, OnInit, Renderer2, ViewChild } from '@angular/core';
import { ProductPhoto } from 'src/app/_models/product';

@Component({
  selector: 'app-image-gallery',
  templateUrl: './image-gallery.component.html',
  styleUrls: ['./image-gallery.component.css']
})
export class ImageGalleryComponent implements OnInit {
  @Input() photos: ProductPhoto[];
  currentPositon: number;
  size: number;
  
  constructor() { 
  }

  ngOnInit(): void {
    this.currentPositon = 0;
    this.size = this.photos.length;
  }

  next() {
    if(this.currentPositon < this.photos.length -1)
      this.currentPositon++;
  }

  previous() {
    if(this.currentPositon > 0)
      this.currentPositon--;
  }

  select(index: number) {
    this.currentPositon = index;
  }
}
