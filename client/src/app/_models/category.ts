export interface SubCategoryCatalogue {
  id: number;
  categoryName: string;
}

export interface CategoryCatalogue {
  id: number;
  categoryName: string;
  slug: string;
  subCategories: SubCategoryCatalogue[];
}

export interface Catalogue {
  gender: number;
  genderTitle: string;
  slug: string;
  categories: CategoryCatalogue[];
}


export enum Gender
{
  Men, 
  Women,
  Unisex, 
  Kid
}