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
        return 'status cancel-requested-status';
      case 5:
        return 'status cancelled-status';
      case 6:
        return 'status shipping-status';
      case 7:
        return 'status shipped-status';
      case 9:
        return 'status finished-status';
      case 10:
        return 'status return-requested-status';
      case 11:
        return 'status returned-status';
      default:
        return 'status default-status';
    }
  }

  export function fnGetUserStateStyle(status: number) {
    switch (status) {
      case 0:
        return 'status active-status';
      case 1:
        return 'status hidden-status';
      default:
        return 'status default-status';
    }
  }