export class SelectOption {
  value: string;
  id: number;
  isSelected: boolean;
  constructor(id: number,  value: string, isSelected: boolean) {
    this.id = id;
    this.value = value;
    this.isSelected = isSelected;
  }
}

export class ConfirmResult {
  result: boolean;
  reason: string;
  constructor(result: boolean, reason: string) {
    this.result = result;
    this.reason = reason;
  }
}

export class DialogResult {
  result: boolean;
  constructor(result: boolean,) {
    this.result = result;
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

export class MultipleSelectedResult {
  result: boolean;
  optionsSelected: OptionSelected[];
  constructor(result: boolean, optionsSelected: OptionSelected[]) {
    this.result = result;
    this.optionsSelected = optionsSelected;
  }
}

export class OptionSelected {
  value: string;
  id: number;

  constructor(id: number, value: string) {
    this.value = value;
    this.id = id;
  }
}