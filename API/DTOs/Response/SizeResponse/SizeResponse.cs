using System;
using API.Entities;

namespace API.DTOs.Response.SizeResponse
{
    public class SizeResponse
    {
        public int Id { get; set; }
        public string SizeName { get; set; }
        public Status Status { get; set; }
        
    }

    public class AdminSizeResponse : SizeResponse
    {

    }

    public class AdminSizeDetailResponse : SizeResponse
    {
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHidden { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
       
    }
}