export class ProductParams {
    category = "";
    gender = "";
    brand = "";
    pageNumber = 1;
    pageSize = 12;
    orderBy = 1;
    field = "DateCreated";
    query = "";
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