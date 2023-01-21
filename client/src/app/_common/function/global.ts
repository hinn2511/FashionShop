import { FormGroup } from '@angular/forms';
import { OrderStatusList } from 'src/app/_models/order';

export function calculatePreviewOffset(
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

export function fnGetObjectStateStyle(state: number): string {
  switch (state) {
    case 0:
      return 'status active-status';
    case 1:
      return 'status hidden-status';
    case 2:
      return 'status deleted-status';
    default:
      return 'status default-status';
  }
}

export function fnGetObjectStateString(state: number): string {
  switch (state) {
    case 0:
      return 'Active';
    case 1:
      return 'Hidden';
    case 2:
      return 'Deleted';
    default:
      return 'Not defined';
  }
}

export function fnGetOrderStateStyle(status: number) {
  switch (status) {
    case 0:
      return 'status created-status';
    case 1:
      return 'status checking-status';
    case 2:
      return 'status paid-status';
    case 3:
      return 'status processing-status';
    case 4:
      return 'status shipping-status';
    case 5:
      return 'status cancelled-status';
    default:
      return 'status default-status';
  }
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
  var result = [];
  array.forEach(function (a) {
      result.push(a);
      if (Array.isArray(a.children)) {
          result = result.concat(fnFlattenArray(a.children));
      }
  });
  return result;
}

// getArrayDepth = arr => {

//   return Array.isArray(arr) ? 1 + Math.max(...arr.map(this.getArrayDepth)) : typeof(arr) === 'object' ? Math.max(...Object.values(arr).map(this.getArrayDepth)): 0;
// };
