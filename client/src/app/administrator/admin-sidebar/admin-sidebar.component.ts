import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { animate, animateChild, AUTO_STYLE, group, query, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-admin-sidebar',
  templateUrl: './admin-sidebar.component.html',
  styleUrls: ['./admin-sidebar.component.css'],
  animations: [
    trigger('collapse', [
      state('false', style({ width: AUTO_STYLE, visibility: AUTO_STYLE})),
      state('true', style({ width: '0', visibility: 'hidden'})),
      transition('false => true', animate('500ms ease-in')),
      transition('true => false', animate('500ms ease-out'))
    ])
  ]
})
export class AdminSidebarComponent implements OnInit {
  @Input() show: boolean;

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  hasRoute(route: string) {
    return this.router.url.includes(route);
}

}
