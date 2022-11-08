import { CustomerCarousel } from './../_models/carousel';
import { ContentService } from 'src/app/_services/content.service';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AnimationType } from '../_common/animation/carousel.animations';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {
  animationType: AnimationType.Fade;
  carousels: CustomerCarousel[] = [];

  constructor(private contentService: ContentService, private router: Router) { }

  ngOnInit(): void {  
    this.loadCarousels();
  }

  loadCarousels()
  {
    this.contentService.getCustomerCarousels().subscribe(result => 
      {
        this.carousels = result;
        for(const carousel of this.carousels)
        {
          new Image().src = carousel.imageUrl; 
        }
      });
  }

  hasRoute(route: string) {
    return this.router.url.includes(route);
  }

}
