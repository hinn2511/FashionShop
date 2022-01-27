import { Component, Input, OnInit } from '@angular/core';
import { BreadCrumb } from 'src/app/_models/breadcrum';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit {
  @Input() breadCrumbList: BreadCrumb[];
  lastBreadCrumb: BreadCrumb;

  constructor() { }

  ngOnInit(): void {
    this.lastBreadCrumb = this.breadCrumbList.pop();
  }

}
