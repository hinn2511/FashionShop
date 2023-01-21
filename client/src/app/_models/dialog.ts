export class ConfirmResult {
  result: boolean;
  reason: string;
  constructor(result: boolean, reason: string) {
    this.result = result;
    this.reason = reason;
  }
}

export class SingleSelectedResult {
  result: boolean;
  selectedValue: string;
  selectedId: number;
  constructor(result: boolean, selectedId: number,  selectedValue: string) {
    this.result = result;
    this.selectedId = selectedId;
    this.selectedValue = selectedValue;
  }
}