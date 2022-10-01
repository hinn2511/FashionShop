export interface ProductPhoto {
    isMain: boolean;
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