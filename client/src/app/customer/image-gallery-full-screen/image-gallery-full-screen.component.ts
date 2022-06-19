import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PhotoService } from 'src/app/_services/photo.service';
import { take } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-image-gallery-full-screen',
  templateUrl: './image-gallery-full-screen.component.html',
  styleUrls: ['./image-gallery-full-screen.component.css']
})
export class ImageGalleryFullScreenComponent implements OnInit {
  imageUrl: string;
  @Output() newItemEvent = new EventEmitter<string>();
  @Output() close = new EventEmitter<boolean>();
  
  constructor(public photoService: PhotoService) {
  
  }

  ngOnInit(): void {
    // this.photoService.changeEmitted$.subscribe(url => {
    //   console.log(url);
    //   this.imageUrl = url;
    //  });
    // this.photoService.getCurrentPhoto().subscribe(result => {
    //   this.imageUrl = result
    // });
  }

  closeImage() {
    this.photoService.clearChange();
    this.close.emit(true);
  }

}
