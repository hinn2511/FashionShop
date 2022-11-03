import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Catalogue, Gender } from '../_models/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

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
    return Gender[+(localStorage.getItem("selectedGender"))];
  }

  getCatalogue() {
    return this.http.get<Catalogue[]>(this.baseUrl + 'category');
  }

}
