import { CreateOption, UpdateOption, ManagerOptionColor, ManagerOptionSize, CustomerOption } from 'src/app/_models/productOptions';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IdArray } from 'src/app/_models/adminRequest';
import { ManagerOption, ManagerOptionParams } from 'src/app/_models/productOptions';
import { getPaginatedResult, getPaginationHeaders } from '../_helpers/paginationHelper';
import { ResponseMessage } from '../_models/generic';

@Injectable({
  providedIn: 'root'
})
export class OptionService {
  baseUrl = environment.apiUrl;
  managerOptionParams: ManagerOptionParams;

  constructor(private http: HttpClient) {
    this.managerOptionParams = new ManagerOptionParams();
  }

  public getSelectedOptionId(): number {
    return +(localStorage.getItem("selectedOptionId"));
  }

  public setSelectedOptionId(value: number) {
    localStorage.setItem("selectedOptionId", value.toString())
  }

  public removeSelectedOptionId() {
    localStorage.removeItem("selectedOptionId")
  }

  getOptionParams() {
    return this.managerOptionParams;
  }

  setOptionParams(params: ManagerOptionParams) {
    this.managerOptionParams = params;
  }

  resetOptionParams() {
    this.managerOptionParams = new ManagerOptionParams();
    return this.managerOptionParams;
  }


  getManagerOptions(optionParams: ManagerOptionParams) {
    let params = getPaginationHeaders(optionParams.pageNumber, optionParams.pageSize);
    params = params.append('orderBy', optionParams.orderBy);
    params = params.append('field', optionParams.field);
    params = params.append('query', optionParams.query);
    optionParams.productOptionStatus.forEach(element => {
      params = params.append('productOptionStatus', element);
    });

    optionParams.productIds.forEach(element => {
      params = params.append('productIds', element);
    });

    return getPaginatedResult<ManagerOption[]>(this.baseUrl + 'productOption/all', params, this.http);
  }

  getManagerOption(id: number) {
    return this.http.get<ManagerOption>(this.baseUrl + 'productOption/' + id + '/detail');
  }

  addOption(option: CreateOption) {
    return this.http.post<CreateOption>(this.baseUrl + 'productOption/create', option);
  }

  editOption(id: number, option: UpdateOption) {
    return this.http.put<UpdateOption>(this.baseUrl + 'productOption/edit/' + id, option);
  }

  hideOptions(ids: IdArray) {
    return this.http.put<ResponseMessage>(this.baseUrl + 'productOption/hide', ids);
  }

  activateOptions(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'productOption/activate',
      ids
    );
  }

  deleteOption(ids: number[]) {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: {
        ids
      },
    };
    return this.http.delete(this.baseUrl + 'productOption/soft-delete', options);
  }
  
  getManagerColorOption() {
    return this.http.get<ManagerOptionColor[]>(this.baseUrl + 'color/all');
  }

  getManagerSizeOption() {
    return this.http.get<ManagerOptionSize[]>(this.baseUrl + 'size/all');
  }

  getCustomerProductOption(productId: number) {
    return this.http.get<CustomerOption[]>(this.baseUrl + 'productOption/' + productId);
  }

}
