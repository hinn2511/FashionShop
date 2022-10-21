import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/_services/authentication.service';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {
  collapseNavbar: boolean = true;
  collapseSearchbar: boolean = true;

  constructor(private authenticationService: AuthenticationService) { }

  ngOnInit(): void {
  }

  navigationBarToggle() {
    this.collapseNavbar = !this.collapseNavbar;
    console.log(this.collapseNavbar);
  }

  searchBarToggle() {
    this.collapseSearchbar = !this.collapseSearchbar;
  }

  logout() {
    this.authenticationService.logout();
  }

  isUserExist(): boolean {
    return this.authenticationService.userValue !== null && this.authenticationService.userValue !== undefined;
  }

}
