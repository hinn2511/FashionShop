export function calculatePreviewOffset(photoQuantity: number, maxItem: number, index: number): [number, number] {
    let newLeftOffset = 0;
    let newRightOffset = 0;
    index++;

    if (photoQuantity < maxItem) {
      newLeftOffset = 0;
      newRightOffset = maxItem;
      return [newLeftOffset, newRightOffset]; 
    }

    if (index <= maxItem) {
      newLeftOffset = 0;
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


  export function getObjectStateStyle(state: number): string {
    switch (state) {
      default:
        return 'width: 100px ;background-color: rgb(20, 20, 255)';
      case 0:
        return 'width: 100px ;background-color: rgb(51, 155, 51)';
      case 1:
        return 'width: 100px ;background-color: rgb(124, 124, 124)';
      case 2:
        return 'width: 100px ;background-color: rgb(155, 51, 51)';
    }
  }

  export function getObjectState(state: number): string {
    switch (state) {
      default:
        return 'New';
      case 0:
        return 'Active';
      case 1:
        return 'Hidden';
      case 2:
        return 'Deleted';
    }
  }
