import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { User } from './_models/user';
import { AuthenticationService } from './_services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    user: User;

    constructor(private authenticationService: AuthenticationService, private router: Router) {
        this.authenticationService.user.subscribe(x => this.user = x);
    }
    ngOnInit(): void {
        if(localStorage.getItem("user") != null || localStorage.getItem("user") != undefined)
        {
            this.authenticationService.setUser();
        }
    }

    logout() {
        this.authenticationService.logout();
    }

    hasRoute(route: string) {
        return this.router.url.includes(route);
    }

   
}
