import { RouteService } from 'src/app/_services/route.service';
import { Carousel } from 'src/app/_models/carousel';
import { ContentService } from 'src/app/_services/content.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { AnimationType } from 'src/app/_common/animation/carousel.animations';
import { Subscription } from 'rxjs';
import { AuthenticationService } from 'src/app/_services/authentication.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css'],
})
export class HomePageComponent implements OnInit, OnDestroy {
  animationType: AnimationType.Fade;
  carousels: Carousel[] = [];
  routeSubscription$: Subscription;
  currentRoute: string = '';

  constructor(
    private contentService: ContentService,
    private routeService: RouteService,
    public authenticationService: AuthenticationService
  ) {}

  ngOnDestroy(): void {
    this.routeSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.loadCarousels();
    this.currentRouteSubscribe();
  }

  loadCarousels() {
    this.contentService.getCustomerCarousels().subscribe((result) => {
      this.carousels = result;
      for (const carousel of this.carousels) {
        new Image().src = carousel.imageUrl;
      }
    });
  }

  currentRouteSubscribe() {
    this.routeSubscription$ = this.routeService.route$.subscribe(() => {
      this.currentRoute = this.routeService.currentRoute;
    });
  }
}
