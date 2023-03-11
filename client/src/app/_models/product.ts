export interface ProductPhoto {
    id: number;
    url: string;
    publicId: string;
    fileType: number;
}

export interface Product {
    isPromoted: boolean;
    isNew: boolean;
    isOnSale: boolean;
    saleType: number;
    saleOffPercent:number;
    saleOffValue: number;
    saleOffFrom: Date;
    saleOffTo: Date;
    category: string;
    categoryId: number;
    parentCategory: string;
    parentCategoryId: number;
    brandId: number;
    brandName: string;
    likedByUser: boolean;
    productPhotos: ProductPhoto[];
    productName: string;
    slug: string;
    price: number;
    description: string;
    url: string;
    gender: number;
    id: number;
    options: ProductColorOption[];
    
}

export class ProductColorOption {
    colorName: string;
    colorCode: string;
}

export interface ManagerProductPhoto {
    status: number;
    id: number;
    isMain: boolean;
    url: string;
    publicId: string;
}

export interface SelectedProductPhoto {
    status: number;
    id: number;
    isMain: boolean;
    url: string;
    publicId: string;
    checked: boolean;
}



export interface ManagerProduct {
    categoryId: number;
    categoryName: string;
    categoryGender: number;
    brandId: number;
    brandName: string;
    productPhotos: ManagerProductPhoto[];
    dateCreated: Date;
    createdByUserId: number;
    lastUpdated: Date;
    lastUpdatedByUserId: number;
    dateDeleted: Date;
    deletedByUserId: number;
    isDeleted: boolean;
    isHidden: boolean;
    dateHidden: Date;
    hiddenByUserId: number;
    status: number;
    productName: string;
    slug: string;
    price: number;
    url: string;
    id: number;
    description: string;
    isPromoted: boolean;
}



export interface SubCategory {
    categoryName: string;
    categoryId: number;
    gender: number;
    id: number;
}

export interface Brand {
    name: string;
    id: number;
}

export interface AddProduct {
    productName: string;
    categoryId: number;
    subCategoryId?: any;
    brandId: number;
    price: number;
    description: string;
}

export interface UpdateProduct {
    productName: string;
    categoryId: number;
    subCategoryId?: any;
    brandId: number;
    price: number;
    description: string;
}

export interface PhotoViewerItem {
    url: string;
    fileType: number;
}