import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { IdArray } from 'src/app/_models/adminRequest';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { AddArticle, CustomerArticle, CustomerArticleDetail, CustomerArticleParams, ManagerArticle, ManagerArticleParams, UpdateArticle } from 'src/app/_models/article';
import { getPaginatedResult, getPaginationHeaders } from 'src/app/_helpers/paginationHelper';
import { map } from 'rxjs/operators';
import { of } from 'rxjs';
import { ResponseMessage } from '../_models/generic';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {

  baseUrl = environment.apiUrl;
  managerArticleParams: ManagerArticleParams;
  customerArticleParams: CustomerArticleParams;

  customerArticleCache = new Map();

  constructor(
    private http: HttpClient,
    private authenticationService: AuthenticationService
  ) {
    this.managerArticleParams = new ManagerArticleParams();
    this.customerArticleParams = new CustomerArticleParams();
  }


  getManagerArticleParams() {
    return this.managerArticleParams;
  }

  setManagerArticleParams(params: ManagerArticleParams) {
    this.managerArticleParams = params;
  }

  resetManagerArticleParams() {
    this.managerArticleParams = new ManagerArticleParams();
    return this.managerArticleParams;
  }

  getManagerArticles(articleParams: ManagerArticleParams) {
    let params = getPaginationHeaders(
      articleParams.pageNumber,
      articleParams.pageSize
    );
    params = params.append('orderBy', articleParams.orderBy);
    params = params.append('field', articleParams.field);
    params = params.append('query', articleParams.query);
    articleParams.
    articleStatus.forEach((element) => {
      params = params.append('articleStatus', element);
    });

    articleParams.contentTypes.forEach((element) => {
      params = params.append('contentTypes', element);
    });

    return getPaginatedResult<ManagerArticle[]>(
      this.baseUrl + 'article/all',
      params,
      this.http
    );
  }

  getManagerArticle(id: number) {
    return this.http.get<ManagerArticle>(
      this.baseUrl + 'article/' + id + "/detail"
    );
  }

  addArticle(article: AddArticle) {
    return this.http.post<ResponseMessage>(
      this.baseUrl + 'article/create',
      article
    );
  }

  editArticle(id: number, article: UpdateArticle) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'article/edit/' + id,
      article
    );
  }

  hideArticles(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'article/hide',
      ids
    );
  }

  activateArticles(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'article/activate',
      ids
    );
  }

  setEditorChoice(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'article/set-editor-choice',
      ids
    );
  }

  removeEditorChoice(ids: IdArray) {
    return this.http.put<ResponseMessage>(
      this.baseUrl + 'article/remove-editor-choice',
      ids
    );
  }

  deleteArticle(ids: number[]) {
    const options = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      }),
      body: {
        ids,
      },
    };
    return this.http.delete<ResponseMessage>(
      this.baseUrl + 'article/soft-delete',
      options
    );
  }

  getCustomerArticleParams() {
    return this.customerArticleParams;
  }

  setCustomerArticleParams(params: CustomerArticleParams) {
    this.customerArticleParams = params;
  }

  resetCustomerArticleParams() {
    this.customerArticleParams = new CustomerArticleParams();
    return this.managerArticleParams;
  }

  getCustomerArticles(articleParams: CustomerArticleParams) {
    let params = getPaginationHeaders(
      articleParams.pageNumber,
      articleParams.pageSize
    );
    params = params.append('orderBy', articleParams.orderBy);
    params = params.append('field', articleParams.field);

    return getPaginatedResult<CustomerArticle[]>(
      this.baseUrl + 'article',
      params,
      this.http
    );
  }

  getCustomerTrendingArticles(top: number) {

    return this.http.get<CustomerArticle[]>(
      this.baseUrl + 'article/most-viewed?top=' + top
    );
  }

  getCustomerEditorChoiceArticles() {

    return this.http.get<CustomerArticle[]>(
      this.baseUrl + 'article/editor-choice'
    );
  }

  getCustomerArticle(id: number) {

    const article = [...this.customerArticleCache.values()]
      .find((article: CustomerArticle) => article.id === id);
      
    if (article) {
      return of(article);
    }
    return this.http.get<CustomerArticleDetail>(
      this.baseUrl + 'article/' + id
    ).pipe(
      map(response => {
        this.customerArticleCache.set(Object.values(id), response);
        setTimeout(_ => { this.customerArticleCache.delete(id)}, 60 * 60 * 1000);
        return response;
      })
    );
  }
}
