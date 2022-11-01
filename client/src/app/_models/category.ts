export interface SubCategoryCatalogue {
  id: number;
  categoryName: string;
}

export interface CategoryCatalogue {
  id: number;
  categoryName: string;
  subCategories: SubCategoryCatalogue[];
}

export interface Catalogue {
  gender: number;
  genderTitle: string;
  categories: CategoryCatalogue[];
}
