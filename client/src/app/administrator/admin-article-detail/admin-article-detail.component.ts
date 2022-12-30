import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { fnGetObjectStateString, fnGetObjectStateStyle } from 'src/app/_common/function/global';
import { IdArray } from 'src/app/_models/adminRequest';
import { ManagerArticle, ArticleTypeList } from 'src/app/_models/article';
import { ArticleService } from 'src/app/_services/article.service';

@Component({
  selector: 'app-admin-article-detail',
  templateUrl: './admin-article-detail.component.html',
  styleUrls: ['./admin-article-detail.component.css']
})
export class AdminArticleDetailComponent implements OnInit {
    article: ManagerArticle;
    type: string;
    id: IdArray;

    constructor(private articleService: ArticleService, private router: Router, private route: ActivatedRoute, private toastr: ToastrService) { }
  
    ngOnInit(): void {

      const articleId = this.route.snapshot.paramMap.get('id');
      this.id = {
        ids: [+articleId]
      };
      this.loadArticleDetail(+articleId);
    }
  
    loadArticleDetail(articleId: number) {
      this.articleService.getManagerArticle(articleId).subscribe(result => {
        this.article = result;
        this.type = this.convertToArticleTypeString();
      })
    }
  
    getArticleState() {
      return fnGetObjectStateString(this.article.status);
    }
  
    getArticleStateStyle() {
      return fnGetObjectStateStyle(this.article.status);
    }
  
    editArticle() {
      this.router.navigateByUrl(
        '/administrator/article-manager/edit/' + this.article.id
      );
    }
  
    hideArticle() {    
      
      this.articleService.hideArticles(this.id).subscribe((result) => {
        this.loadArticleDetail(this.article.id);
        this.toastr.success('Article have been hidden or unhidden', 'Success');
      }, 
      error => 
      {
        this.toastr.error("Something wrong happen!", 'Error');
      });
    }
  
    deleteArticle() {
      this.articleService.deleteArticle(this.id.ids).subscribe((result) => {
        this.loadArticleDetail(this.article.id);
        this.toastr.success('Article have been deleted', 'Success');
      }, 
      error => 
      {
        this.toastr.error("Something wrong happen!", 'Error');
      });
    }


    convertToArticleTypeString()
    {
      return ArticleTypeList.find(x => x.id == this.article.contentType).name;
    }
  }