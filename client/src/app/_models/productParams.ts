export class ProductParams {
  categoryId = 0;
  colorCode =  '';
  size = '';
  gender = 0;
  brand = '';
  pageNumber = 1;
  pageSize = 12;
  orderBy = 1;
  field = 'Sold';
  query = '';
  minPrice = 0;
  maxPrice = 0;
  isOnSale = false;
  isMostInteresting = false;
  isFeatured = false;
}

export class CustomerFilterOrder {
  id: number;
  field: string;
  orderBy: number;
  filterName: string;

  constructor(id: number, field: string, orderBy: number, filterName: string)
  {
    this.id = id;
    this.field = field;
    this.orderBy = orderBy;
    this.filterName = filterName;
  }
}

export class CustomerPriceRange {
  id: number;
  minPrice: number;
  maxPrice: number;
  priceRangeString: string;

  constructor(id: number, minPrice: number, maxPrice: number) {
    this.id = id;
    this.minPrice = minPrice;
    this.maxPrice = maxPrice;
    if (this.minPrice < 0) this.priceRangeString = `Less than ${this.maxPrice}$`;
    if (this.maxPrice < 0) this.priceRangeString = `More than ${this.minPrice}$`;
    if (this.minPrice >= 0 && this.maxPrice >= 0)
      this.priceRangeString = `From ${this.minPrice}$ to ${this.maxPrice}$`;
  }
}

export class CustomerSizeFilter {
  id: number;
  sizeName: string;
  constructor(id: number, sizeName: string) {
    this.id = id;
    this.sizeName = sizeName;
  }
}

export class CustomerColorFilter {
  id: number;
  colorName: string;
  colorCode: string;
  constructor(id: number, colorName: string , colorCode: string) {
    this.id = id;
    this.colorName = colorName;
    this.colorCode = colorCode;
  }
}


export class ManagerProductParams {
  category = '';
  gender = '';
  brand = '';
  pageNumber = 1;
  pageSize = 12;
  orderBy = 0;
  field = 'Id';
  query = '';
  productStatus = [0, 1];
}

export interface ParamStatus {
  status: number;
  selected: boolean;
}
