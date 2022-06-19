import { Component, OnInit } from '@angular/core';
import { PhotoService } from './_services/photo.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  showImage: boolean;

  constructor(private photoService: PhotoService){
    photoService.changeEmitted$.subscribe(text => {
      if(text !== null) {
        this.showFullScreenImageToggle();
      }
    });
  }

  ngOnInit(): void {
    this.showImage = false;
  }

  showFullScreenImageToggle() {
    this.showImage = !this.showImage;
  }
}
