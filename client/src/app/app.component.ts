import { animate, animateChild, AUTO_STYLE, group, query, state, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from './_models/user';
import { AuthenticationService } from './_services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  animations: [
    trigger('collapse', [
        state('false', style({ width: AUTO_STYLE, visibility: AUTO_STYLE})),
      state('true', style({ width: '0', visibility: 'hidden'})),
      transition('false => true', animate('500ms ease-in')),
      transition('true => false', animate('500ms ease-out'))
    ]),
    trigger('rotatedState', [
        state('default', style({ transform: 'rotate(0)' })),
        state('rotated', style({ transform: 'rotate(-180deg)' })),
        transition('rotated => default', animate('500ms ease-out')),
        transition('default => rotated', animate('500ms ease-in'))
      ]),
      trigger('slideInOut', [
        state(
          'in',
          style({
            transform: 'translate3d(-12vw,0,0)',
          })
        ),
        state(
          'out',
          style({
            transform: 'translate3d(0, 0, 0)',
          })
        ),
        transition('in => out', animate('400ms ease-in-out')),
        transition('out => in', animate('400ms ease-in-out')),
      ]),
  ]
})
export class AppComponent implements OnInit {
    user: User;
    showSidebar: boolean;

    state: string = 'default';

    menuState: string = 'in';

    constructor(private authenticationService: AuthenticationService, private router: Router) {
      if(localStorage.getItem("user") !== null || localStorage.getItem("user") !== undefined)
      {
          this.authenticationService.setUser();
          this.authenticationService.user.subscribe(x => this.user = x);
      }
    }
    ngOnInit(): void {
       
        this.showSidebar = true;
    }

    logout() {
        this.authenticationService.logout();
    }

    hasRoute(route: string) {
        return this.router.url.includes(route);
    }

    // sideBarToggle()
    // {
    //     this.showSidebar = !this.showSidebar;
    // }

    toggleMenu() {
        this.menuState = this.menuState === 'out' ? 'in' : 'out';
    }

    rotate() {
        this.state = (this.state === 'default' ? 'rotated' : 'default');
    }
}
