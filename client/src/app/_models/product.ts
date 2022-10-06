export interface ProductPhoto {
    isMain: boolean;
    isHidden: boolean;
    id: number;
    url: string;
    publicId: string;
}

export interface Product {
    id: number;
    productName: string;
    slug: string;
    categoryId: number;
    brandId: number;
    price: number;
    sold: number;
    url: string;
    description: string;
    productPhotos: ProductPhoto[];
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
    productPrice: number;
    gender: string;
}

export interface EditProduct {
    id: number;
    productName: string;
    categoryId: number;
    productPrice: number;
    gender: string;
}