import { GenericObject } from './generic';
import { Params } from './params';

export class Gender extends GenericObject {
  constructor(id: number, name: string) {
    super(id, name);
  }
}

export interface CustomerCategoryCatalogue {
  subCategories: CustomerCategoryCatalogue[];
  slug: string;
  categoryName: string;
  parentId: number;
  gender: number;
  id: number;
}

export interface CustomerCatalogue {
  gender: number;
  genderTitle: string;
  categories: CustomerCategoryCatalogue[];
}

export class Category {
  id: number;
  categoryName: string;
  categoryImageUrl: string;
  gender: number;
  parent: Category;
  slug: string;
}

export interface ManagerCategoryCatalogue {
  subCategories: ManagerCategoryCatalogue[];
  slug: string;
  categoryName: string;
  parentId: number;
  gender: number;
  id: number;
}

export interface ManagerCatalogue {
  gender: number;
  genderTitle: string;
  categories: ManagerCategoryCatalogue[];
}

export class ManagerCategory extends Category {
  genderName: string;
  status: number;
  isPromoted: boolean;
  gender: number;
}

export class ManagerSubCategory extends Category {
  status: number;
}

export class AddCategory {
  categoryName: string;
  categoryImageUrl: string;
  parentId: number;
  gender: number;
}
export class UpdateCategory {
  categoryName: string;
  categoryImageUrl: string;
  parentId: number;
  gender: number;
}

export class UpdateSubCategory extends Category {
  categoryName: string;
  categoryImageUrl: string;
  parentCategoryId: number;
  gender: number;
}


export interface SubCategory {
  id: number;
  categoryName: string;
  status: number;
}

export interface ManagerCategoryDetail extends Category {
  dateCreated: Date;
  createdByUserId: number;
  lastUpdated: Date;
  lastUpdatedByUserId: number;
  dateDeleted: Date;
  deletedByUserId: number;
  dateHidden: Date;
  hiddenByUserId: number;
  subCategories: SubCategory[];
  genderName: string;
  status: number;
  id: number;
  isPromoted: boolean;
  slug: string;
  categoryName: string;
  parentId: number;
  gender: number;
  categoryImageUrl: string;
  parentCategory: string;
}


export class ManagerCategoryParams extends Params {
  categoryStatus: number[];
  genders: number[];
  parentId: number;
 
}

export const GenderList = [
  new Gender(0, 'Men'),
  new Gender(1, 'Women'),
  new Gender(2, 'Unisex'),
  new Gender(3, 'Kid'),
];

export function fnGetGenderName(id: number) {
  return GenderList.find((x) => x.id == id).name;
}
