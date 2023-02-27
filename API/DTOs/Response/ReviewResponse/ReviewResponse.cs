using System;
using System.Collections.Generic;
using API.Entities.OrderModel;

namespace API.DTOs.Response.ReviewResponse
{
    #region base response
    public class BaseReviewResponse
    {
        public DateTime DateCreated { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
    }
    #endregion


    #region customer
    public class CustomerReviewResponse : BaseReviewResponse
    {
        public string UserName { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
    }

    public class CustomerReviewedItemResponse : CustomerReviewResponse
    {
        public string ProductName { get; set; }
        public string Url { get; set; }
    }

    public class CustomerProductReviewSummary 
    {
        public CustomerProductReviewSummary(double averageScore, int total, List<ProductReviewScore> scores)
        {
            AverageScore = averageScore;
            Total = total;
            Scores = scores;
        }

        public double AverageScore { get; set; }
        public int Total { get; set; }
        public List<ProductReviewScore> Scores { get; set; }
    }

    public class ProductReviewScore
    {
        public ProductReviewScore(int score, int count)
        {
            Score = score;
            Count = count;
        }

        public int Score { get; set; }
        public int Count { get; set; }
    }

    #endregion
}