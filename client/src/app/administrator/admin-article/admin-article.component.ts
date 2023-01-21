import { Component, OnInit } from '@angular/core';
import { RotateAnimation } from 'src/app/_common/animation/carousel.animations';
import { ArticleService } from 'src/app/_services/article.service';
import { Pagination } from 'src/app/_models/pagination';
import { Router } from '@angular/router';
import { IdArray } from 'src/app/_models/adminRequest';
import { ToastrService } from 'ngx-toastr';
import { ManagerArticle, ManagerArticleParams, ArticleType, ArticleTypeList } from 'src/app/_models/article';
import { GenericStatus, GenericStatusList } from 'src/app/_models/generic';
import { fnGetObjectStateString, fnGetObjectStateStyle } from 'src/app/_common/function/style-class';


@Component({
  selector: 'app-admin-article',
  templateUrl: './admin-article.component.html',
  styleUrls: ['./admin-article.component.css'],
  animations: [RotateAnimation]
})
export class AdminArticleComponent implements OnInit {
  articles: ManagerArticle[];
  pagination: Pagination;
  articleParams: ManagerArticleParams;
  isSelectAllArticle: boolean;
  state: string = 'default';
  showStatusFilter: boolean;
  showContentTypeFilter: boolean;
  selectedIds: number[] = [];
  articleTypes: ArticleType[] = ArticleTypeList;
  genericStatus: GenericStatus[] = GenericStatusList;
  query: string;

  constructor(
    private articleService: ArticleService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.articleParams = this.articleService.getManagerArticleParams();
  }

  ngOnInit(): void {
    this.articleParams.field = 'Id';
    this.articleParams.orderBy = 0;
    this.articleParams.articleStatus = [0, 1];
    this.isSelectAllArticle = false;
    this.showStatusFilter = false;
    this.loadArticles();
  }

  rotate() {
    this.state = this.state === 'default' ? 'rotated' : 'default';
  }

  loadArticles() {
    this.articleService.setManagerArticleParams(this.articleParams);

    this.articleService
      .getManagerArticles(this.articleParams)
      .subscribe((response) => {
        this.articles = response.result;
        this.pagination = response.pagination;
      });
  }

  resetSelectedIds() {
    this.selectedIds = [];
  }

  viewDetail(articleId: number) {
    this.router.navigateByUrl(
      '/administrator/article-manager/detail/' + articleId
    );
  }

  editArticle() {
    if (!this.isSingleSelected()) return;
    this.router.navigateByUrl(
      '/administrator/article-manager/edit/' + this.selectedIds[0]
    );
  }

  hideArticles() {
    // if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.articleService.hideArticles(ids).subscribe(
      (result) => {
        this.loadArticles();
        this.resetSelectedIds();
        this.toastr.success(
          result.message,
          'Success'
        );
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  activateArticles() {
    // if (!this.isMultipleSelected()) return;
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.articleService.activateArticles(ids).subscribe(
      (result) => {
        this.loadArticles();
        this.resetSelectedIds();
        this.toastr.success(
          result.message,
          'Success'
        );
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  setEditorChoice() {
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.articleService.setEditorChoice(ids).subscribe(
      (result) => {
        this.loadArticles();
        this.resetSelectedIds();
        this.toastr.success(
          result.message,
          'Success'
        );
      },
      (error) => {
        this.toastr.error(error.message, 'Error');
      }
    );
  }

  removeEditorChoice() {
    let ids: IdArray = {
      ids: this.selectedIds,
    };

    this.articleService.removeEditorChoice(ids).subscribe(
      (result) => {
        this.loadArticles();
        this.resetSelectedIds();
        this.toastr.success(
          result.message,
          'Success'
        );
      },
      (error) => {
        this.toastr.error(error.message, 'Error');
      }
    );
  }

  deleteArticles() {
    if (!this.isMultipleSelected()) return;
    this.articleService.deleteArticle(this.selectedIds).subscribe(
      (result) => {
        this.loadArticles();
        this.resetSelectedIds();
        this.toastr.success('Product articles have been deleted', 'Success');
      },
      (error) => {
        this.toastr.error('Something wrong happen!', 'Error');
      }
    );
  }

  pageChanged(event: any) {
    if (this.articleParams.pageNumber !== event.page) {
      this.articleParams.pageNumber = event.page;
      this.articleService.setManagerArticleParams(this.articleParams);
      this.loadArticles();
    }
  }

  sort(type: number) {
    this.articleParams.orderBy = type;
    this.loadArticles();
  }

  filter(params: ManagerArticleParams) {
    this.articleParams = params;
    this.loadArticles();
  }

  resetFilter() {
    this.articleParams = this.articleService.resetManagerArticleParams();
    this.loadArticles();
  }

  orderBy(field: string) {
    switch (field) {
      case 'id':
        this.articleParams.field = 'Id';
        break;
      case 'headline':
        this.articleParams.field = 'Headline';
        break;
      case 'headlineSlug':
        this.articleParams.field = 'HeadlineSlug';
        break;
      case 'publishedBy':
        this.articleParams.field = 'PublishedBy';
        break;
      case 'publishedDate':
        this.articleParams.field = 'PublishedDate';
        break;
      case 'status':
        this.articleParams.field = 'Status';
        break;
      case 'view':
          this.articleParams.field = 'View';
          break;
      case 'promoted':
          this.articleParams.field = 'Promoted';
          break;
      default:
        this.articleParams.field = 'PublishedDate';
        break;
    }
    if (this.articleParams.orderBy == 0) this.articleParams.orderBy = 1;
    else this.articleParams.orderBy = 0;
    this.rotate();
    this.loadArticles();
  }

  selectAllArticles() {
    if (this.isSelectAllArticle) {
      this.selectedIds = [];
    } else {
      this.selectedIds = this.articles.map(({ id }) => id);
    }
    this.isSelectAllArticle = !this.isSelectAllArticle;
  }

  selectArticle(id: number) {
    if (this.selectedIds.includes(id)) {
      this.selectedIds.splice(this.selectedIds.indexOf(id), 1);
    } else {
      this.selectedIds.push(id);
    }
  }

  isArticleSelected(id: number) {
    return this.selectedIds.indexOf(id) >= 0;
  }

  getArticleState(article: ManagerArticle) {
    return fnGetObjectStateString(article.status);
  }

  getStateStyle(article: ManagerArticle) {
    return fnGetObjectStateStyle(article.status);
  }

  isAllStatusIncluded() {
    return this.articleParams.articleStatus.length == this.genericStatus.length;
  }

  isStatusIncluded(status: number) {
    return this.articleParams.articleStatus.indexOf(status) > -1;
  }

  selectStatus(status: number) {
    if (this.isStatusIncluded(status))
      this.articleParams.articleStatus =
        this.articleParams.articleStatus.filter((x) => x !== status);
    else this.articleParams.articleStatus.push(status);
    this.articleParams.articleStatus = [
      ...this.articleParams.articleStatus,
    ].sort((a, b) => a - b);
    this.loadArticles();
  }

  selectAllArticleStatus() {
    if (this.isAllStatusIncluded())
      this.articleParams.articleStatus = [];
    else this.articleParams.articleStatus = this.genericStatus.map(x => x.id);
    this.loadArticles();
  }

  statusFilterToggle() {
    this.showStatusFilter = !this.showStatusFilter;
  }

  isAllContentTypeIncluded() {
    return this.articleParams.contentTypes.length == this.articleTypes.length;
  }

  isContentTypeIncluded(status: number) {
    return this.articleParams.contentTypes.indexOf(status) > -1;
  }

  selectContentType(status: number) {
    if (this.isContentTypeIncluded(status))
      this.articleParams.contentTypes =
        this.articleParams.contentTypes.filter((x) => x !== status);
    else this.articleParams.contentTypes.push(status);
    this.articleParams.contentTypes = [
      ...this.articleParams.contentTypes,
    ].sort((a, b) => a - b);
    this.loadArticles();
  }

  selectAllArticleContentType() {
    if (this.isAllContentTypeIncluded())
      this.articleParams.contentTypes = [];
    else this.articleParams.contentTypes = this.articleTypes.map(x => x.id);
    this.loadArticles();
  }

  contentTypeFilterToggle() {
    this.showContentTypeFilter = !this.showContentTypeFilter;
  }

  isSingleSelected() {
    return this.selectedIds.length == 1;
  }

  isMultipleSelected() {
    return this.selectedIds.length >= 1;
  }
}
