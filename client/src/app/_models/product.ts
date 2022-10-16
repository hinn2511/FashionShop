
export interface ProductPhoto {
    id: number;
    url: string;
    publicId: string;
}

export interface Product {
    categoryId: number;
    categoryName: string;
    subCategoryName?: string;
    subCategoryId?: number;
    brandId: number;
    brandName: string;
    likedByUser: boolean;
    productPhotos: ProductPhoto[];
    productName: string;
    slug: string;
    price: number;
    url: string;
    id: number;
    description: string;
}

export interface ManagerProductPhoto {
    status: string;
    id: number;
    isMain: boolean;
    url: string;
    publicId: string;
}

export interface SelectedProductPhoto {
    status: string;
    id: number;
    isMain: boolean;
    url: string;
    publicId: string;
    checked: boolean;
}

export interface ManagerProduct {
    categoryId: number;
    categoryName: string;
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
}


export interface Category {
    categoryName: string;
    gender: number;
    id: number;
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