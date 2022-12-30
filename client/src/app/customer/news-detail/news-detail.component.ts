import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from 'src/app/_services/article.service';
import { ArticleTypeList, CustomerArticle, CustomerArticleDetail } from 'src/app/_models/article';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-news-detail',
  templateUrl: './news-detail.component.html',
  styleUrls: ['./news-detail.component.css']
})
export class NewsDetailComponent implements OnInit {
  article: CustomerArticleDetail;
  trendingArticles: CustomerArticle[] = [];
  contentTypeString: string = "";
  constructor(private articleService: ArticleService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    window.scroll({ 
      top: 0, 
      left: 0, 
      behavior: 'smooth' 
  });
    this.loadArticle(this.route.snapshot.queryParams['id']);
    this.loadTrendingArticles();
  }
  
  loadArticle(id: number)
  { 
    this.articleService.getCustomerArticle(id).subscribe(response => {
      this.article = response;
      this.getContentTypeString();
    })
  }

  loadTrendingArticles()
  { 
    this.articleService.getCustomerTrendingArticles(3).subscribe(response => {
      this.trendingArticles = response;
    })
  }

  getContentTypeString()
  {
    this.contentTypeString = ArticleTypeList.find(x => x.id == this.article.contentType).name;
  }

}
