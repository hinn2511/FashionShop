import { ToastrService } from 'ngx-toastr';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { Subscription } from 'rxjs';
import { SettingService } from 'src/app/_services/setting.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit, OnDestroy {
  customerRegisterBackgroundUrl: string = "";
  registerForm: FormGroup;
  loading = false;
  submitted = false;
  confirmPasswordSubscription$: Subscription;

  constructor(
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private settingService: SettingService,
    private router: Router,
    private authenticationService: AuthenticationService
  ) {
    if (this.authenticationService.userValue) {
      this.router.navigate(['/']);
    }
  }
  ngOnDestroy(): void {
    this.confirmPasswordSubscription$.unsubscribe();
  }

  ngOnInit() {
    this.customerRegisterBackgroundUrl = this.settingService.settingValue.clientRegisterBackground;
    new Image().src = this.customerRegisterBackgroundUrl;
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', [Validators.required,
      this.matchValues('password')]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      email: ['', Validators.required],
      gender: ['', Validators.required],
      dateOfBirth: [new Date(), Validators.required]
    });

    this.confirmPasswordSubscription$ = this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
    }
  }

  onSubmit() {
    this.submitted = true;
    this.loading = true;    
    this.authenticationService.register(this.registerForm.value).subscribe(result => 
      {
        this.toastr.success("Success", result.message);
        this.loading = false;
        this.router.navigateByUrl('/login');
      },
      error => 
      {
        this.toastr.success("Error", error.message);
        this.loading = false;
      });
  }

}
