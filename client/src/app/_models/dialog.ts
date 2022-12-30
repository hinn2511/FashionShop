export class ConfirmResult {
  result: boolean;
  reason: string;
  constructor(result: boolean, reason: string) {
    this.result = result;
    this.reason = reason;
  }
}
