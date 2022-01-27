import { AfterViewInit, Component, ElementRef, Input, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { ButtonImageClickDirective } from 'src/app/_directives/button-image-click.directive';
import { ProductPhoto } from 'src/app/_models/product';

@Component({
  selector: 'app-image-gallery',
  templateUrl: './image-gallery.component.html',
  styleUrls: ['./image-gallery.component.css']
})
export class ImageGalleryComponent implements OnInit {
  @Input() photos: ProductPhoto[];
  @Input() mainPhotoUrl: string;
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
