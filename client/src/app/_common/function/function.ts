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
