import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css']
})
export class NavigationBarComponent implements OnInit {
  collapseNavbar: boolean = true;
  collapseSearchbar: boolean = true;

  constructor() { }

  ngOnInit(): void {
  }

  navigationBarToggle() {
    this.collapseNavbar = !this.collapseNavbar;
    console.log(this.collapseNavbar);
  }

  searchBarToggle() {
    this.collapseSearchbar = !this.collapseSearchbar;
  }

}
