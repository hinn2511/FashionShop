import { ArticleService } from 'src/app/_services/article.service';
import { Component, OnInit } from '@angular/core';
import {
  ArticleTypeList,
  CustomerArticle,
  CustomerArticleParams,
} from 'src/app/_models/article';
import { Pagination } from 'src/app/_models/pagination';
import { ActivatedRoute, Router } from '@angular/router';
import { CustomerFilterOrder } from 'src/app/_models/productParams';
import { CustomerCarousel } from 'src/app/_models/carousel';

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.css'],
})
export class NewsComponent implements OnInit {
  articles: CustomerArticle[];
  editorChoiceArticles: CustomerArticle[];
  editorChoiceSlides: CustomerCarousel[] = [];
  trendingArticles: CustomerArticle[];

  skeletonItems: number[];
  skeletonLoading: boolean = false;
  loadingCount: number = 0;
  pagination: Pagination;
  customerArticleParams: CustomerArticleParams;

  // filter selected value
  filterOrders: CustomerFilterOrder[] = [
    new CustomerFilterOrder(0, 'Date', 1, 'Newest'),
    new CustomerFilterOrder(1, 'View', 1, 'Most view'),
    new CustomerFilterOrder(2, 'View', 1, 'Most view'),
  ];

  selectedOrder: string;
  selectedCategory: string;
  selectedGender: number;
  selectedGenderString: string;

  constructor(
    private articleService: ArticleService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.customerArticleParams = this.articleService.getCustomerArticleParams();
  }

  ngOnInit(): void {
    this.skeletonLoading = true;
    this.skeletonItems = Array(this.customerArticleParams.pageSize).fill(1);
    this.customerArticleParams.field = 'DatePublished';
    this.customerArticleParams.orderBy = 1;
    this.customerArticleParams.pageSize = 9;
    this.loadCustomerArticles();
    this.loadCustomerEditorChoiceArticles();
    this.loadCustomerTrendingArticles();
  }

  loadCustomerArticles() {
    if (this.loadingCount == 0) this.skeletonLoading = true;
    // this.skeletonLoading = true;
    this.articleService.setCustomerArticleParams(this.customerArticleParams);

    this.articleService
      .getCustomerArticles(this.customerArticleParams)
      .subscribe((response) => {
        if (this.loadingCount == 0) {
          this.loadingCount++;
          setTimeout(() => {
            this.skeletonLoading = false;
          }, 500);
          // this.skeletonLoading = false;
        }
        this.articles = response.result;
        this.pagination = response.pagination;
      });
  }

  loadCustomerEditorChoiceArticles() {
    this.articleService
      .getCustomerEditorChoiceArticles()
      .subscribe((response) => {
        this.editorChoiceArticles = response;
        this.editorChoiceSlides = [];
        this.editorChoiceArticles.forEach(element => {
          this.editorChoiceSlides.push(new CustomerCarousel(element.headline, element.foreword, element.headlineSlug, element.thumbnailUrl));
        });

        
      });
  }

  loadCustomerTrendingArticles() {
    this.articleService
      .getCustomerTrendingArticles(5)
      .subscribe((response) => {
        this.trendingArticles = response;
      });
  }

  pageChanged(event: any) {
    if (this.customerArticleParams.pageNumber !== event.page) {
      this.customerArticleParams.pageNumber = event.page;
      this.articleService.setCustomerArticleParams(this.customerArticleParams);
      this.loadCustomerArticles();
    }
  }

  sort(type: number) {
    let filterOrder = this.filterOrders[type];
    this.selectedOrder = filterOrder.filterName;
    this.customerArticleParams.orderBy = filterOrder.orderBy;
    this.customerArticleParams.field = filterOrder.field;
    this.loadCustomerArticles();
  }

  filter(params: CustomerArticleParams) {
    this.customerArticleParams = params;
    this.loadCustomerArticles();
  }

  resetFilter() {
    this.customerArticleParams =
      this.articleService.resetCustomerArticleParams();
    this.loadCustomerArticles();
  }

  getContentTypeString(article: CustomerArticle)
  {
    return ArticleTypeList.find(x => x.id == article.contentType).name;
  }

  viewDetail(article: CustomerArticle) {
    this.router.navigate(
      ['news/' + article.headlineSlug],
      { queryParams: { id: article.id } }
    );
  }
}
