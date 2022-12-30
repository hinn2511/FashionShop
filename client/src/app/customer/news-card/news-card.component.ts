import { Router } from '@angular/router';
import { CustomerArticle, ArticleTypeList } from 'src/app/_models/article';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-news-card',
  templateUrl: './news-card.component.html',
  styleUrls: ['./news-card.component.css']
})
export class NewsCardComponent implements OnInit {
  @Input() article: CustomerArticle;
  contentTypeString: string;

  showForeword: boolean = false;
  constructor(private router: Router) { }

  ngOnInit(): void {
    this.getContentTypeString();
  }

  getContentTypeString()
  {
    this.contentTypeString = ArticleTypeList.find(x => x.id == this.article.contentType).name;
  }

  setForeword(show: boolean)
  {
    if (this.article.foreword == null && show)
      return;
    this.showForeword = show;
  }

  viewDetail() {
    this.router.navigate(
      ['news/' + this.article.headlineSlug],
      { queryParams: { id: this.article.id } }
    );
  }

}
