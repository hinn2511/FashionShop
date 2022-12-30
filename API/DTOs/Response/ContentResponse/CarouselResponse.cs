using System;
using API.Entities;
using API.Entities.WebPageModel;

namespace API.DTOs.Response.ContentResponse
{
    public class BaseCarouselResponse
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        
    }

    public class CustomerCarouselResponse : BaseCarouselResponse
    {
    }
    
    public class AdminCarouselResponse : BaseCarouselResponse
    {
        public int Id { get; set; }
        public Status Status { get; set; }
    }

    public class AdminCarouselDetailResponse : BaseCarouselResponse
    {
        public int Id { get; set; }
        public Status Status { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public double AdditionalPrice { get; set; }
    }

    
}