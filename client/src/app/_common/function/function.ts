import { FormGroup } from '@angular/forms';
import { OrderStatusList } from 'src/app/_models/order';

export function fnCalculatePreviewOffset(
  photoQuantity: number,
  maxItem: number,
  index: number
): [number, number] {
  let newLeftOffset = 0;
  let newRightOffset = 0;
  index++;

  if (photoQuantity < maxItem) {
    newRightOffset = maxItem;
    return [newLeftOffset, newRightOffset];
  }

  if (index <= maxItem) {
    newRightOffset = maxItem;
    return [newLeftOffset, newRightOffset];
  }

  if (index > photoQuantity - maxItem) {
    newLeftOffset = photoQuantity - maxItem;
    newRightOffset = photoQuantity;
    return [newLeftOffset, newRightOffset];
  }

  if (maxItem % 2 == 0) {
    newLeftOffset = index - maxItem / 2;
    newRightOffset = newLeftOffset + maxItem + 1;
  } else {
    newLeftOffset = index - maxItem / 2;
    newRightOffset = newLeftOffset + maxItem;
  }
  return [newLeftOffset, newRightOffset];
}

export function fnGetOrderStateString(status: number) {
  return OrderStatusList.find((x) => x.id == status).statusString;
}

export function fnUpdateFormControlStringValue(
  formGroup: FormGroup,
  controlName: string,
  newValue: string,
  emitEvent: boolean
) {
  formGroup.controls[controlName].setValue(newValue, { emitEvent: emitEvent });
}

export function fnUpdateFormControlNumberValue(
  formGroup: FormGroup,
  controlName: string,
  newValue: number,
  emitEvent: boolean
) {
  formGroup.controls[controlName].setValue(newValue, { emitEvent: emitEvent });
}

export function fnGetFormControlValue(
  formGroup: FormGroup,
  controlName: string
) {
  return formGroup.controls[controlName].value;
}

export function fnGetArrayDepth(arr) {
  if (Array.isArray(arr)) return 1 + Math.max(...arr.map(fnGetArrayDepth));
  else {
    if (typeof arr === 'object')
      return Math.max(...Object.values(arr).map(fnGetArrayDepth));
    else return 0;
  }
}

export function fnFlattenArray(array) {
  let result = [];
  array.forEach(function (a) {
    result.push(a);
    if (Array.isArray(a.children)) {
      result = result.concat(fnFlattenArray(a.children));
    }
  });
  return result;
}

export function fnConvertToSlug(text: string) {
  return text
    .toLowerCase()
    .replace(/ /g, '-')
    .replace(/[^\w-]+/g, '');
}

export function fnCalculatePrice(
  saleType: number,
  price: number,
  saleOffPercent: number,
  saleOffValue: number
) {  
  switch (saleType)
  {
    case 1:
      return price - (price * saleOffPercent) / 100;
    case 2:
      return price - saleOffValue
    default:
      return price;
  }
}

export function fnHasValue<T>(object: T): boolean
{
  return object != null && object != undefined;
}

export function fnIsNullOrEmpty(object: string): boolean
{
  return object == '' || !fnHasValue<string>(object);
}


export function fnSwitchValue<T>(object: T, trueValue: T, falseValue: T): T
{
  if ( object == trueValue)
    return falseValue;
  else
    return trueValue;

}


export function fngetReturnUrl(roles: string[]) {
  if (!fnHasValue<string[]>(roles))
    return '/';

  if (fnHasValue(roles.find((x) => x === 'AdministratorAccess'))) {
    if (fnHasValue(roles.find((x) => x === 'DashboardAccess'))) {
      return '/administrator/dashboard';
    }
    if (fnHasValue(roles.find((x) => x === 'ProductManagerAccess'))) {
      return '/administrator/product-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'OrderManagerAccess'))) {
      return '/administrator/order-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'CategoryManagerAccess'))) {
      return '/administrator/category-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'ProductOptionManagerAccess'))) {
      return '/administrator/option-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'SettingManagerAccess'))) {
      return '/administrator/setting-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'RoleManagerAccess'))) {
      return '/administrator/role-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'UserManagerAccess'))) {
      return '/administrator/user-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'ArticleManagerAccess'))) {
      return '/administrator/article-manager';
    }
    if (fnHasValue(roles.find((x) => x === 'CarouselManagerAccess'))) {
      return '/administrator/carousel-manager';
    }

  }
  return '/';
}

export function fnIsLightColor(color: string) {
  const hex = color.toLowerCase().replace('#', '');
  const c_r = parseInt(hex.substring(0, 0 + 2), 16);
  const c_g = parseInt(hex.substring(2, 2 + 2), 16);
  const c_b = parseInt(hex.substring(4, 4 + 2), 16);
  const brightness = ((c_r * 299) + (c_g * 587) + (c_b * 114)) / 1000;
  return brightness > 155;
}