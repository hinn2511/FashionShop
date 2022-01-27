import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Category } from '../_models/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  baseUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }

  getCategories(gender: string) {
    return this.http.get<Category[]>(this.baseUrl + 'category/' + gender);
  }
}
