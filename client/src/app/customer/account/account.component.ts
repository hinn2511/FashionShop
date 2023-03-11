import { SlideRightToLeft } from './../../_common/animation/common.animation';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { fnHasValue } from 'src/app/_common/function/function';

export class AccountTab {
  name: string;
  queryParam: string;
  constructor(name: string, queryParam: string) {
    this.name = name;
    this.queryParam = queryParam;
  }
}

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css'],
  animations: [ SlideRightToLeft ]
})
export class AccountComponent implements OnInit {
  activeTab: AccountTab;

  tabs: AccountTab[] = [
    new AccountTab('Account information', 'information'),
    new AccountTab('My orders', 'orders'),
    new AccountTab('Favorites', 'favorites'),
  ];

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.activeTab = this.tabs[0];
    this.route.data.subscribe((data) => {
      this.route.queryParams.subscribe((params) => {
        let tab = this.tabs.find((x) => x.queryParam == params.tab);
        if (fnHasValue<AccountTab>(tab)) this.activeTab = tab;
        else this.activeTab = this.tabs[0];
        
      });
    });
  }

  selectTab(param: string) {
    this.router.navigate(['account/'], { queryParams: { tab: param } });
  }
}
