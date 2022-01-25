export interface Product {
    id: number;
    productName: string;
    slug: string;
    category: string;
    gender: string;
    productPrice: number;
    sold: number;
    stocks: any[];
    productPhotos: any[];
}