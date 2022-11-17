export interface CartItem {
    id: number;
    optionId: number;
    productId: number;
    productName: string;
    price: number;
    colorName: string;
    colorCode: string;
    sizeName: string;
    additionalPrice: number;
    quantity: number;
    totalItemPrice: number;
    imageUrl: string;
}

export interface CartItemList {
    cartItems: CartItem[];
    totalPrice: number;
    totalItem: number;
}


export interface NewCartItem
{
    optionId: number;
    quantity: number;
}

export interface UpdateCartItem
{
    cartId: number;
    quantity: number;
}

export interface UpdateCartItemAfterLogin
{
    newCartItems: NewCartItem[];
}