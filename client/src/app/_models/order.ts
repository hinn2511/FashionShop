import { GenericObject } from './generic';
//Generic
export class ShippingMethod extends GenericObject {
  minDate: number;
  maxDate: number;
  price: number;

  constructor(
    id: number,
    name: string,
    minDate: number,
    maxDate: number,
    price: number
  ) {
    super(id, name);
    this.minDate = minDate;
    this.maxDate = maxDate;
    this.price = price;
  }
}

export const ShippingMethodList = [
  new ShippingMethod(0, 'Fast delivery', 1, 3, 30),
  new ShippingMethod(1, 'Normal delivery', 3, 5, 20),
  new ShippingMethod(2, 'Economic delivery', 5, 7, 10),
];

export class PaymentMethod extends GenericObject {
  constructor(id: number, name: string) {
    super(id, name);
  }
}

export const PaymentMethodList = [
  new PaymentMethod(0, 'Credit card'),
  new PaymentMethod(1, 'Debit card'),
  new PaymentMethod(2, 'Cash on delivery'),
  new PaymentMethod(3, 'Mobile payments'),
];

export class OrderStatusFilter {
  ids: number[];
  statusString: string;
  constructor(ids: number[], statusString: string) {
    this.ids = ids;
    this.statusString = statusString;
  }
}

export const CustomerStatusFilters = [
  new OrderStatusFilter([0, 1, 4], 'Checking'),
  new OrderStatusFilter([2, 3, 4], 'Processing'),
  new OrderStatusFilter([6], 'Shipping'),
  new OrderStatusFilter([5, 7, 8, 9, 10], 'Finished'),
  new OrderStatusFilter([11], 'Returned'),
];

export enum OrderStatusEnum {
  Created,
  Checking,
  Paid,
  Processing,
  CancelRequested,
  Cancelled,
  Shipping,
  Shipped,
  Declined,
  Finished,
  ReturnRequested,
  Returned,
}

export const allowVerifyStatus: OrderStatusEnum[] = [
  OrderStatusEnum.Created,
  OrderStatusEnum.Paid,
  OrderStatusEnum.Checking,
];

export const allowShippingStatus: number[] = [OrderStatusEnum.Processing];

export const allowShippedStatus: number[] = [OrderStatusEnum.Shipping];

export const allowCancelStatus: number[] = [
  OrderStatusEnum.Created,
  OrderStatusEnum.Paid,
  OrderStatusEnum.Checking,
  OrderStatusEnum.Processing,
  OrderStatusEnum.Shipping,
];

export const allowCancelRequestStatus: number[] = [
  OrderStatusEnum.Created,
  OrderStatusEnum.Paid,
  OrderStatusEnum.Checking,
  OrderStatusEnum.Processing,
];

export const allowCancelAcceptStatus: number[] = [
  OrderStatusEnum.CancelRequested,
];

export const allowReturnRequestStatus: number[] = [OrderStatusEnum.Shipped];

export const allowConfirmDeliveredStatus: number[] = [OrderStatusEnum.Shipped];

export const allowReturnAcceptStatus: number[] = [
  OrderStatusEnum.ReturnRequested,
];

export const allowReviewStatus: number[] = [
  OrderStatusEnum.Finished,
];

export class OrderStatus {
  id: number;
  statusString: string;

  constructor(id: number, statusString: string) {
    this.id = id;
    this.statusString = statusString;
  }
}

export class OrderStatusSummary extends OrderStatus{
  total: number;

  constructor(id: number, statusString: string, total: number) {
    super(id, statusString);
    this.total = total;
  }
}

export const OrderStatusList = [
  new OrderStatus(0, 'Created'),
  new OrderStatus(2, 'Paid'),
  new OrderStatus(1, 'Checking'),
  new OrderStatus(3, 'Processing'),
  new OrderStatus(6, 'Shipping'),
  new OrderStatus(7, 'Shipped'),
  new OrderStatus(9, 'Finished'),
  new OrderStatus(8, 'Declined'),
  new OrderStatus(4, 'Cancel Requested'),
  new OrderStatus(5, 'Cancelled'),
  new OrderStatus(10, 'Return Requested'),
  new OrderStatus(11, 'Returned'),
];

export const OrderStatusSummaryList = [
  new OrderStatusSummary(0, 'Created', 0),
  new OrderStatusSummary(2, 'Paid', 0),
  new OrderStatusSummary(1, 'Checking', 0),
  new OrderStatusSummary(3, 'Processing', 0),
  new OrderStatusSummary(6, 'Shipping', 0),
  new OrderStatusSummary(7, 'Shipped', 0),
  new OrderStatusSummary(9, 'Finished', 0),
  new OrderStatusSummary(8, 'Declined', 0),
  new OrderStatusSummary(4, 'Cancel Requested', 0),
  new OrderStatusSummary(5, 'Cancelled', 0),
  new OrderStatusSummary(10, 'Return Requested', 0),
  new OrderStatusSummary(11, 'Returned', 0),
];

export class OrderRequest {
  reason: string;
  constructor(reason: string) {
    this.reason = reason;
  }
}

export class CancelOrderRequest extends OrderRequest {}

export class ReturnOrderRequest extends OrderRequest {}


export const isAllowVerify = (id: number) =>
  allowVerifyStatus.find((x) => x === id) != undefined;

export const isAllowShipping = (id: number) =>
  allowShippingStatus.find((x) => x === id) != undefined;

export const isAllowShipped = (id: number) =>
  allowShippedStatus.find((x) => x === id) != undefined;

export const isAllowCancelRequest = (id: number) =>
  allowCancelRequestStatus.find((x) => x === id) != undefined;

export const isAllowCancelAccept = (id: number) =>
  allowCancelAcceptStatus.find((x) => x === id) != undefined;

export const isAllowCancel = (id: number) =>
  allowCancelStatus.find((x) => x === id) != undefined;

export const isAllowReturnRequest = (id: number) =>
  allowReturnRequestStatus.find((x) => x === id) != undefined;

export const isAllowReturnAccept = (id: number) =>
  allowReturnAcceptStatus.find((x) => x === id) != undefined;

export const isAllowConfirmDelivered = (id: number) =>
  allowConfirmDeliveredStatus.find((x) => x === id) != undefined;

export const isAllowReview = (id: number) =>
  allowReviewStatus.find((x) => x === id) != undefined;


// Customer
export class CustomerNewOrder {
  receiverName: string;
  address: string;
  phoneNumber: string;
  email: string;
  shippingMethod: number;
  paymentMethod: number;
  isFromCart: boolean;
}

export class CustomerCardInformation {
  cardHolder: string;
  cardNumber: string;
  cVV: string;
  expiredDate: string;
}

export class CustomerOrderParams {
  pageNumber = 1;
  pageSize = 10;
  orderBy = 1;
  orderStatusFilter = [];
  field = '';
  query = '';
  from = new Date();
  to = new Date();
}

export interface CustomerOrderHistory {
  note: string;
  dateCreated: Date;
  orderStatus: number;
}

export interface CustomerOrderDetail {
  id: number;
  productId: number;
  productName: string;
  optionId: number;
  price: number;
  quantity: number;
  total: number;
  isReviewed: boolean;
}


export interface CustomerOrder {
  orderHistories: CustomerOrderHistory[];
  orderDetails: CustomerOrderDetail[];
  externalId: string;
  dateCreated: Date;
  paymentMethod: number;
  currentStatus: number;
  currentStatusString: string;
  receiverName: string;
  address: string;
  phoneNumber: string;
  email: string;
  subTotal: number;
  tax: number;
  shippingMethod: string;
  shippingFee: number;
  totalItem: number;
  totalPrice: number;
  isFinished: boolean;
}

// Manager
export class ManagerOrderParams {
  pageNumber = 1;
  pageSize = 10;
  orderBy = 1;
  orderStatusFilter: number[] = [];
  field = '';
  query = '';
  from = new Date();
  to = new Date();
  paymentMethodFilter = [0, 1, 2, 3];
  shippingMethodFilter = [
    'Fast delivery',
    'Normal delivery',
    'Economic delivery',
  ];
}

export interface ManagerOrderSummary {
  orderStatus: number;
  total: number;
}

export interface ManagerOrderHistory {
  id: number;
  historyDescription: string;
  dateCreated: Date;
  orderStatus: number;
}

export interface ManagerOrderDetail {
  id: number;
  productId: number;
  productName: string;
  price: number;
  url: string;
  colorName: string;
  colorCode: string;
  sizeName: string;
  quantity: number;
  total: number;
  stockAvailable: number;
  stockAfterDeduction: number;
}

export interface ManagerOrder {
  id: number;
  createdByUserId: number;
  lastUpdated: Date;
  lastUpdatedByUserId: number;
  dateDeleted: Date;
  deletedByUserId: number;
  dateHidden: Date;
  hiddenByUserId: number;
  orderHistories: ManagerOrderHistory[];
  orderDetails: ManagerOrderDetail[];
  receiverName: string;
  address: string;
  phoneNumber: string;
  email: string;
  subTotal: number;
  tax: number;
  shippingMethod: string;
  shippingFee: number;
  externalId: string;
  dateCreated: Date;
  paymentMethod: number;
  currentStatus: number;
  paymentMethodString: string;
  currentStatusString: string;
  totalItem: number;
  totalPrice: number;
}

