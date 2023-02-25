import { RotateAnimation } from './../../_common/animation/carousel.animations';
import { concatMap, catchError } from 'rxjs/operators';
import { DeviceService } from 'src/app/_services/device.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import {
  Component,
  EventEmitter,
  OnInit,
  Output,
} from '@angular/core';
import { CartItem } from 'src/app/_models/cart';
import { User } from 'src/app/_models/user';
import { CartService } from 'src/app/_services/cart.service';
import { Subscription } from 'rxjs';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  CustomerNewOrder,
  CustomerCardInformation,
  PaymentMethod,
  ShippingMethod,
  ShippingMethodList,
  PaymentMethodList,
} from 'src/app/_models/order';
import { OrderService } from 'src/app/_services/order.service';

@Component({
  selector: 'app-check-out',
  templateUrl: './check-out.component.html',
  styleUrls: ['./check-out.component.css'],
  animations: [ RotateAnimation ],
})
export class CheckOutComponent implements OnInit {
  @Output() hideCheckout = new EventEmitter<boolean>();
  @Output() backToCart = new EventEmitter<boolean>();
  shippingMethods: ShippingMethod[] = ShippingMethodList;
  selectedShippingMethod: ShippingMethod;
  paymentMethods: PaymentMethod[] = PaymentMethodList;
  selectedPaymentMethod: PaymentMethod;
  orderDetailForm: FormGroup;
  order: CustomerNewOrder;
  paymentInformation: CustomerCardInformation;
  checkingOrder: boolean = false;
  user: User;
  discountAmount: number = 0;
  expandCheckoutSummary: boolean = false;
  deviceType: string = '';
  deviceSubscription$: Subscription;

  cardNumberSubscribe$: Subscription;
  cvvSubscribe$: Subscription;
  expiredDateSubscribe$: Subscription;
  cardHolderSubscribe$: Subscription;

  constructor(
    private authenticationService: AuthenticationService,
    public cartService: CartService,

    private orderService: OrderService,
    private toastr: ToastrService,
    private router: Router,
    private deviceService: DeviceService,
    private fb: FormBuilder
  ) {}

  ngOnDestroy(): void {
    this.deviceSubscription$.unsubscribe();
  }

  ngOnInit(): void {
    this.selectedShippingMethod = this.shippingMethods[0];
    this.selectedPaymentMethod = this.paymentMethods[0];
    this.initializeForm();
    this.cardSubscribe();
    this.updateOrder();
    this.updatePaymentInformation();
    this.order.shippingMethod = this.selectedShippingMethod.id;
    this.order.paymentMethod = this.selectedPaymentMethod.id;
    this.deviceSubscription$ = this.deviceService.deviceWidth$.subscribe(
      (_) => {
        this.deviceType = this.deviceService.getDeviceType();
        if (this.deviceType == 'desktop') {
          this.expandCheckoutSummary = true;
        }
      }
    );
    this.user = this.authenticationService.userValue;
  }

  initializeForm() {    
    this.orderDetailForm = this.fb.group({
      receiverName: ['', Validators.required],
      address: ['', [Validators.required]],
      phoneNumber: ['', [Validators.required]],
      email: [''],
      cardHolder: [''],
      cardNumber: ['', [Validators.minLength(19), Validators.maxLength(19)]],
      cvv: ['', [Validators.minLength(3), Validators.maxLength(3),Validators.pattern('[0-9]{3}'),]],
      expiredDate: [
        '',
        [
          Validators.minLength(5),
          Validators.maxLength(5),
          Validators.pattern('(0[1-9]|1[0-2])/[0-9]{2}'),
        ],
      ],
      shippingMethod: [this.selectedShippingMethod.id, [Validators.required]],
      paymentMethod: [this.selectedPaymentMethod.id, [Validators.required]],
    });
    this.orderDetailForm.get('paymentMethod').valueChanges.subscribe((val) => {
      if (
        this.selectedPaymentMethod.id == 0 ||
        this.selectedPaymentMethod.id == 1
      ) {
        this.cardSubscribe();
        this.orderDetailForm.controls['cardNumber'].setValidators([
          Validators.required,
          Validators.minLength(19),
          Validators.maxLength(19),
        ]);
        this.orderDetailForm.controls['cvv'].setValidators([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(3),
        ]);
        this.orderDetailForm.controls['expiredDate'].setValidators([
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(5),
          Validators.pattern('(0[1-9]|1[0-2])/[0-9]{2}'),
        ]);
      } else {
        this.cardUnsubscribe();
        this.orderDetailForm.controls['cardHolder'].clearValidators();
        this.orderDetailForm.controls['cardHolder'].patchValue('', { emitEvent: false });
        this.orderDetailForm.controls['cardNumber'].clearValidators();
        this.orderDetailForm.controls['cardNumber'].patchValue('', { emitEvent: false });
        this.orderDetailForm.controls['cvv'].clearValidators();
        this.orderDetailForm.controls['cvv'].patchValue('', { emitEvent: false });
        this.orderDetailForm.controls['expiredDate'].clearValidators();
        this.orderDetailForm.controls['expiredDate'].patchValue('', { emitEvent: false });
      }
      this.orderDetailForm.controls['cardHolder'].updateValueAndValidity();
      this.orderDetailForm.controls['cardNumber'].updateValueAndValidity();
      this.orderDetailForm.controls['cvv'].updateValueAndValidity();
      this.orderDetailForm.controls['expiredDate'].updateValueAndValidity();
    });

    
  }

  cardUnsubscribe()
  {
    this.cardNumberSubscribe$.unsubscribe();
    this.cardHolderSubscribe$.unsubscribe();
    this.cvvSubscribe$.unsubscribe();
    this.expiredDateSubscribe$.unsubscribe();
  }

  cardSubscribe()
  {
    this.cardNumberSubscribe$ = this.orderDetailForm
      .get('cardNumber')
      .valueChanges.subscribe((_) => {
        let currentValue = this.orderDetailForm.controls['cardNumber'].value;
        if (currentValue.length > 19)
          currentValue = currentValue.substring(0,19);
        this.orderDetailForm.controls.cardNumber.setValue(
          this.applySplitter(currentValue, 16, 4, ' '),
          { emitEvent: false }
        );
      });

    this.cardHolderSubscribe$ = this.orderDetailForm
      .get('cardHolder')
      .valueChanges.subscribe((_) => {
        let value = this.orderDetailForm.controls['cardHolder'].value;
        this.orderDetailForm.controls.cardHolder.setValue(
          value.toLocaleUpperCase(),{ emitEvent: false }
        );
      });

    this.cvvSubscribe$ = this.orderDetailForm
      .get('cvv')
      .valueChanges.subscribe((_) => {
        let value = this.orderDetailForm.controls['cvv'].value;
        this.orderDetailForm.controls.cvv.setValue(value.substring(0, 3).replace(/\s+/g, '').replace(/\D/g, ''), { emitEvent: false });
      });

    this.expiredDateSubscribe$ = this.orderDetailForm
      .get('expiredDate')
      .valueChanges.subscribe((_) => {
        let currentValue = this.orderDetailForm.controls['expiredDate'].value;
        if (currentValue.length > 5)
          currentValue = currentValue.substring(0, 5);
        this.orderDetailForm.controls.expiredDate.setValue(
          this.applySplitter(currentValue, 4, 2, '/'),
          { emitEvent: false }
        );
      });
  }

  updateOrder() {
    this.order = {
      receiverName: this.orderDetailForm.controls['receiverName'].value,
      address: this.orderDetailForm.controls['address'].value,
      phoneNumber: this.orderDetailForm.controls['phoneNumber'].value,
      email: this.orderDetailForm.controls['email'].value,
      shippingMethod: this.orderDetailForm.controls['shippingMethod'].value,
      paymentMethod: this.orderDetailForm.controls['paymentMethod'].value,
      isFromCart: true,
    };
  }

  updatePaymentInformation() {
    this.paymentInformation = {
      cardHolder:
        this.orderDetailForm.controls['cardHolder'].value.toLocaleUpperCase(),
      cardNumber: this.orderDetailForm.controls['cardNumber'].value,
      cVV: this.orderDetailForm.controls['cvv'].value,
      expiredDate: this.orderDetailForm.controls['expiredDate'].value,
    };
  }

  submit() {
    this.updatePaymentInformation();
    this.updateOrder();

    this.checkingOrder = true;
    if (
      this.selectedPaymentMethod.id == 0 ||
      this.selectedPaymentMethod.id == 1
    ) {
      this.orderService
        .createOrder(this.order)
        .pipe(
          concatMap((result) =>
            this.orderService.payOrderByCard(
              result,
              this.paymentInformation
            )
          ),
          catchError(async (error) => this.toastr.error(error, 'Error'))
        )

        .subscribe(
          (result) => {
            this.toastr.success(
              'Your order has been created successfully!',
              'Success'
            );
              this.closeCheckout();
          this.router.navigate(['order'], {
            queryParams: { id: result },
          });
          this.cartService.clearCart();
            this.checkingOrder = false;
          },
          (error) => {
            this.toastr.error('An error has occurred when we validate your card information. Please check your payment information and try again later in order histories page.', 'Error');
            this.closeCheckout();
            this.cartService.clearCart();
            this.checkingOrder = false;
          }
        );
      return;
    }

    this.orderService.createOrder(this.order).subscribe(
      (result) => {
        this.toastr.success(
          'Your order has been created successfully!',
          'Success'
        );
        this.closeCheckout();
        this.router.navigate(['order'], {
          queryParams: { id: result },
        });
        this.cartService.clearCart();
        this.checkingOrder = false;
      },
      (error) => {
        this.toastr.error(error, 'Error');
        this.checkingOrder = false;
      }
    );
  }

  selectShippingMethod(shippingMethod: ShippingMethod) {
    this.selectedShippingMethod = shippingMethod;
    this.orderDetailForm.controls['shippingMethod'].setValue(shippingMethod.id);
  }

  selectPaymentMethod(paymentMethod: PaymentMethod) {
    this.selectedPaymentMethod = paymentMethod;
    this.orderDetailForm.controls['paymentMethod'].setValue(paymentMethod.id);
  }

  expandCheckoutSummaryToggle() {
    this.expandCheckoutSummary = !this.expandCheckoutSummary;
  }

  isRotate() {
    if (this.expandCheckoutSummary)
      return 'default';
    return 'rotated';
  }

  viewProduct(cartItem: CartItem) {
    this.router.navigate(['product/' + cartItem.slug], {
      queryParams: { id: cartItem.productId },
    });
    this.closeCheckout();
  }

  closeCheckout() {
    this.hideCheckout.emit(true);
  }

  goBackToCart() {
    this.backToCart.emit(true);
  }
  
  applySplitter(
    input: string,
    maxLength: number,
    partLength: number,
    separator: string
  ) {
    let trimmed = input.replace(/\s+/g, '').replace(/\D/g, '');
    if (trimmed.length >= maxLength) return input;
    let temp = [];
    for (let i = 0; i < trimmed.length; i += partLength) {
      temp.push(trimmed.substring(i, i + partLength));
    }

    return temp.join(separator);
  }
}
