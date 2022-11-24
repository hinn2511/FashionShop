import { Component, OnInit, OnChanges, SimpleChanges, AfterViewInit } from '@angular/core';
import { ActivatedRoute, Route, Router } from '@angular/router';
import { Account } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
export interface AccountTab {
  name: string;
  queryParam: string;
}

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css'],
})
export class AccountComponent implements OnInit {
  account: Account;
  activeTab: AccountTab;
  horizontalTab: boolean = false;
  tabs: AccountTab[] = [
    {
      name: 'Account information',
      queryParam: 'information',
    },
    {
      name: 'Shipping address',
      queryParam: 'shipping',
    },
    {
      name: 'My orders',
      queryParam: 'orders',
    },
    {
      name: 'Favorites',
      queryParam: 'favorites',
    },
    {
      name: 'Payment method',
      queryParam: 'payment',
    }
  ];
  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private router: Router
    ) {
      
    }
    
    ngOnInit(): void {
      this.loadAccountInformation();
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
      this.activeTab = this.tabs[0];
      this.route.data.subscribe((data) => {
        this.route.queryParams.subscribe((params) => {
          params.tab
          ? this.activeTab = this.tabs.find(x => x.queryParam == params.tab)
          : this.activeTab = this.tabs[0];
          this.horizontalTab = window.innerHeight < 768 ? true : false;
      });
    });
  }

  selectTab(param: string) {
    this.activeTab = this.tabs.find(x => x.queryParam == param);
  }

  isActive(param: string) {    
    return this.activeTab.queryParam == param;
  }

  loadAccountInformation()
  {
    this.accountService.getAccountInformation().subscribe(result =>
      {
        this.account = result;
      });
  }
}
