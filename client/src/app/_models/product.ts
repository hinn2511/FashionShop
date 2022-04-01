export interface ProductPhoto {
    id: number;
    url: string;
}

export interface Product {
    id: number;
    productName: string;
    slug: string;
    category: string;
    gender: string;
    url: string;
    productPrice: number;
    sold: number;
    stocks: any[];
    productPhotos: ProductPhoto[];
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