import { Router } from '@angular/router';
import { Component, Input, OnInit } from '@angular/core';
import { BreadCrumb } from 'src/app/_models/breadcrumb';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  @Input() breadCrumbList: BreadCrumb[];

  @Input() showBackground: boolean = false;

  constructor(private router: Router) { }

  ngOnInit(): void {
    //Not implemented
  }

  navigatingTo(breadCrumb: BreadCrumb)
  {
    if(breadCrumb.active)
      this.router.navigateByUrl(breadCrumb.route);
  }

}
