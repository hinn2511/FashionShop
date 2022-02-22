import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-product-rating',
  templateUrl: './product-rating.component.html',
  styleUrls: ['./product-rating.component.css']
})
export class ProductRatingComponent implements OnInit {
  reviewPoint: number = 4.5;
  reviewPoints: number[] = [];
  comment = "Aliquam porttitor sed augue vel tincidunt. Nunc non bibendum dui, ut semper libero. Proin sed tellus ipsum. Donec ultricies, ex id aliquet interdum, metus sapien mollis est, non maximus urna ligula sed enim."

  constructor() { }

  ngOnInit(): void {
    this.getReviewStart();
  }

  getReviewStart() {
    let fullStart = (this.reviewPoint - (this.reviewPoint % 1));
    for(let i = 0; i < fullStart ; i++) {
      this.reviewPoints.push(1);
    }
    if(this.reviewPoint % 1 > 0)
      this.reviewPoints.push(0);
  }

}
