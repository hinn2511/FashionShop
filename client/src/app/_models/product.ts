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