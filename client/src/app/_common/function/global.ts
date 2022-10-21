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
