// Customer
export interface CustomerOptionColor {
  id: number;
  colorName: string;
  colorCode: string;
}

export interface CustomerOptionSize {
  id: number;
  sizeName: string;
  optionId: number;
  additionalPrice: number;
}

export interface CustomerOption {
  color: CustomerOptionColor;
  sizes: CustomerOptionSize[];
}

// Manager
export class ManagerOptionParams {
    pageNumber = 1;
    pageSize = 12;
    orderBy = 0;
    field = "Id";
    query = "";
    productOptionStatus = [0, 1];
}

export interface CreateOption {
    productId: number;
    colorId: number;
    sizeId: number;
    additionalPrice: number;
}

export interface UpdateOption {
    colorId: number;
    sizeId: number;
    additionalPrice: number;
}

export interface ManagerOptionColor {
    id: number;
    colorName: string;
    colorCode: string;
}

export interface ManagerOptionSize {
    id: number;
    sizeName: string;
}

export interface ManagerOptionProduct {
  id: number;
  productName: string;
}

export interface ManagerOption {
    id: number;
    color: ManagerOptionColor;
    size: ManagerOptionSize;
    product: ManagerOptionProduct;
    status: number;
    dateCreated: Date;
    createdByUserId: number;
    lastUpdated: Date;
    lastUpdatedByUserId: number;
    dateDeleted: Date;
    deletedByUserId: number;
    dateHidden: Date;
    hiddenByUserId: number;
    additionalPrice: number;
}