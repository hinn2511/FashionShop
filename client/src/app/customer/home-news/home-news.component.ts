import { DeviceService } from 'src/app/_services/device.service';
import { FadeInAndOut } from './../../_common/animation/common.animation';
import { ArticleService } from 'src/app/_services/article.service';
import { Component, OnInit } from '@angular/core';
import { ArticleTypeList, CustomerArticle, CustomerArticleParams } from 'src/app/_models/article';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home-news',
  templateUrl: './home-news.component.html',
  styleUrls: ['./home-news.component.css'],
  animations: [ FadeInAndOut ]
})
export class HomeNewsComponent implements OnInit {
  expandedNew: CustomerArticle;
  news: CustomerArticle[] = [];

  constructor(private articleService: ArticleService, private deviceService: DeviceService, private router: Router) {}

  ngOnInit(): void {
    this.loadNews();
  }

  loadNews() {
    let newsParams: CustomerArticleParams = {
      pageNumber: 1,
      pageSize: 4,
      orderBy: 1,
      field: 'PublishedDate',
      contentTypes: []
    };

    this.articleService.getCustomerArticles(newsParams).subscribe(response => 
      {
        this.news = response.result;
        this.expandedNew = this.news[0];
      })
  }

  getContentType(type: number)
  {
    return ArticleTypeList.find(x => x.id == type).name;
  }

  expand(article: CustomerArticle)
  {
    if(this.deviceService.getDeviceType() == 'desktop')
      this.expandedNew = article;
  }

  viewDetail(article: CustomerArticle) {
    this.router.navigate(
      ['news/' + article.headlineSlug],
      { queryParams: { id: article.id } }
    );
  }
}
