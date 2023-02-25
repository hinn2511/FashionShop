import { SettingService } from './../../_services/setting.service';
import { CartService } from 'src/app/_services/cart.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { concatMap } from 'rxjs/operators';
import { AuthenticationService } from 'src/app/_services/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;

  customerLoginBackgroundUrl: string = "";
  loading = false;
  submitted = false;
  returnUrl: string;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private cartService: CartService,
    private settingService: SettingService,
    private router: Router,
    private authenticationService: AuthenticationService
  ) {
    if (this.authenticationService.userValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit() {
    console.log(this.settingService.settingValue.clientLoginBackground);
    
    this.customerLoginBackgroundUrl = this.settingService.settingValue.clientLoginBackground;
    new Image().src = this.customerLoginBackgroundUrl;
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get f() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;

    this.loading = true;
    // this.authenticationService.login(this.f.username.value, this.f.password.value, 'client')
    //     .pipe(first())
    //     .subscribe({
    //         next: () => {
    //             this.cartService.getAuthenticatedUserCartItems().subscribe(( ) => {});
    //             this.router.navigate([this.returnUrl]);
    //         },
    //         error: error => {
    //             this.error = error;
    //             this.loading = false;
    //         }
    //     });
    this.authenticationService
      .login(this.f.username.value, this.f.password.value)
      .pipe(concatMap((_) => this.cartService.getUserCartItems()))
      .subscribe({
        next: () => {
          if (this.authenticationService.userValue.roles.find(x => x === "ClientAccess") == null)
            this.authenticationService.logout();
          else
            this.router.navigate([this.returnUrl]);
        },
        error: (error) => {
          this.error = error;
          this.loading = false;
        },
      });
  }
}
