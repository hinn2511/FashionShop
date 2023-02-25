export class ReviewParams {
    pageNumber = 1;
    pageSize = 4;
    orderBy = 1;
    field = 'DateCreated';
    score = 0;
  }
  

  export class CustomerReviewParams extends ReviewParams {
  }
  
  export class UserReview
  {
    optionId: number
    comment: string;
    score: number;
    userName: string
  }
  
  export class CreateUserReviewRequest extends UserReview
  {
   
  }
  
  export class EditUserReviewRequest extends UserReview
  {
  }

  export class ProductReview extends UserReview
  {
    dateCreated: Date;
    colorCode: string;
    colorName: string;
    sizeName: string;
  }

  export class OrderReviewedItem extends ProductReview
  {
    url: string;
    productName: string;
  }

  export class Score {
    score: number;
    count: number;
}

export class ReviewSummary {
    averageScore: number;
    total: number;
    scores: Score[];
}