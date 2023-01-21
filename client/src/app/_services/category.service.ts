import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { IdArray } from '../_models/adminRequest';
import { AddCategory, Category, CustomerCatalogue, fnGetGenderName, ManagerCatalogue, ManagerCategory, ManagerCategoryParams, UpdateCategory } from '../_models/category';
import { ResponseMessage } from '../_models/generic';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  baseUrl = environment.apiUrl;
  managerCategoryParams: ManagerCategoryParams;

  constructor(private http: HttpClient) {
    this.managerCategoryParams = new ManagerCategoryParams();
   }

  setCurrentCategory(categoryName: string, gender: number)
  {
    localStorage.setItem("selectedCategory", categoryName);
    localStorage.setItem("selectedGender", gender.toString());
  }

  getCurrentCategory()
  {
    return localStorage.getItem("selectedCategory");
  }

  getCurrentGender()
  {
    // return Gender[+(localStorage.getItem("selectedGender"))];
    return fnGetGenderName(+(localStorage.getItem("selectedGender")));
  }

  getCatalogue() {
    return this.http.get<CustomerCatalogue[]>(this.baseUrl + 'category');
  }

  getManagerCatalogue() {
    return this.http.get<ManagerCatalogue[]>(this.baseUrl + 'category/catalogue');
  }

  getManagerCategoryParams() {
    return this.managerCategoryParams;
  }

  setManagerCategoryParams(params: ManagerCategoryParams) {
    this.managerCategoryParams = params;
  }

  resetManagerCategoryParams() {
    this.managerCategoryParams = new ManagerCategoryParams();
    return this.managerCategoryParams;
  }

  getManagerCategories(categoryParams: ManagerCategoryParams) {
    let params = getPaginationHeaders(
      categoryParams.pageNumber,
      categoryParams.pageSize
    );
    params = params.append('orderBy', categoryParams.orderBy);
    params = params.append('field', categoryParams.field);
    params = params.append('query', categoryParams.query);
    categoryParams.
    categoryStatus.forEach((element) => {
      params = params.append('categoryStatus', element);
    });

    categoryParams.genders.forEach((element) => {
      params = params.append('genders', element);
    });

    return getPaginatedResult<ManagerCategory[]>(
      this.baseUrl + 'category/all',
      params,
      this.http
    );
  }

  getManagerCategory(id: number) {
    return this.http.get<ManagerCategory>(
      this.baseUrl + 'category/detail/' + id
    );
  }

  addCategory(category: AddCategory) {
    return this.http.post<ResponseMessage>(
      this.baseUrl + 'category/create',
      category
    );
  }

  editCategory(id: number, category: UpdateCategory) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'category/edit/' + id,
      category
    );
  }

  deleteCategories(ids: number[]) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'category/soft-delete',
      ids
    );
  }

  hideCategories(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'category/hide',
      ids
    );
  }

  activateCategories(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'category/activate',
      ids
    );
  }

  promoteCategories(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'category/promote',
      ids
    );
  }

  demoteCategories(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'category/demote',
      ids
    );
  }

  getPromotedCategories() {

    return this.http.get<Category[]>(
      this.baseUrl + 'category/promoted'
    );
  }


}
