export class ProductParams {
    category = "";
    sizeId = 0;
    gender = 0;
    brand = "";
    pageNumber = 1;
    pageSize = 12;
    orderBy = 1;
    field = "Sold";
    query = "";    
    minPrice = 0;
    maxPrice = 0;
}

export interface CustomerFilterOrder {
    id: number;
    field: string;
    orderBy: number;
    name: string;
}

export interface CustomerPriceRange {
    id: number;
    minPrice: number;
    maxPrice: number;
  }
  

export class ManagerProductParams {
    category = "";
    gender = "";
    brand = "";
    pageNumber = 1;
    pageSize = 12;
    orderBy = 0;
    field = "Id";
    query = "";
    productStatus = [0, 1];
}

export interface ParamStatus {
    status: number;
    selected: boolean;
}

