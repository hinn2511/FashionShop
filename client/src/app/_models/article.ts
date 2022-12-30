import { GenericObject } from "./generic";

export class Article {
  
  headline: string;
  contentType: number;
  content: string;
  foreword: string;
  thumbnailUrl: string;
}

export class AddArticle extends Article {}
export class UpdateArticle extends Article {}

export interface ManagerArticle {
  id: number;
  headline: string;
  headlineSlug: string;
  foreword: string;
  content: string;
  contentType: number;
  publishedBy: string;
  publishedDate: string;
  thumbnailUrl: string;
  status: number;
  view: number;
  editorChoice: boolean;
}
export interface CustomerArticle {
  id: number;
  headline: string;
  headlineSlug: string;
  foreword: string;
  publishedDate: string;
  publishedBy: string;
  thumbnailUrl: string;
  contentType: number;
}

export interface CustomerArticleDetail extends CustomerArticle {
  content: string;
}

export class ManagerArticleParams {
  pageNumber = 1;
  pageSize = 12;
  orderBy = 0;
  field = 'PublishedDate';
  query = '';
  articleStatus = [0, 1];
  contentTypes = [1];
}

export class CustomerArticleParams {
  pageNumber = 1;
  pageSize = 12;
  orderBy = 0;
  field = 'PublishedDate';
}

export class ArticleType extends GenericObject {

  constructor(id: number, name: string)
  {
      super(id, name);
  }
}

export const ArticleTypeList: ArticleType[] =
[
    new ArticleType(0, 'Promotion'),
    new ArticleType(1, 'Blog'),
    new ArticleType(2, 'Information'),
    new ArticleType(3, 'Announcement'),
];