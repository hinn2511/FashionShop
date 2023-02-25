import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Account } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';

export class AccountTab {
  name: string;
  queryParam: string;
  constructor(name: string, queryParam: string) {
    (this.name = name), (this.queryParam = queryParam);
  }
}

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css'],
})
export class AccountComponent implements OnInit {
  account: Account;
  activeTab: AccountTab;

  tabs: AccountTab[] = [
    new AccountTab('Account information', 'information'),
    new AccountTab('My orders', 'orders'),
    new AccountTab('Favorites', 'favorites'),
  ];

  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAccountInformation();
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.activeTab = this.tabs[0];
    this.route.data.subscribe((data) => {
      this.route.queryParams.subscribe((params) => {
        params.tab
          ? (this.activeTab = this.tabs.find((x) => x.queryParam == params.tab))
          : (this.activeTab = this.tabs[0]);
      });
    });
  }

  selectTab(param: string) {
    this.router.navigate(['account/'], { queryParams: { tab: param } });
  }

  isActive(param: string) {
    return this.activeTab.queryParam == param;
  }

  loadAccountInformation() {
    this.accountService.getAccountInformation().subscribe((result) => {
      this.account = result;
    });
  }
}
