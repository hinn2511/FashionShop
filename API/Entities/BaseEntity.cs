using System;

namespace API.Entities
{
    public class BaseEntity
    {   
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int LastUpdatedByUserId { get; set; }
        public DateTime DateDeleted { get; set; }
        public int DeletedByUserId { get; set; }
        public DateTime DateHidden { get; set; }
        public int HiddenByUserId { get; set; }
        public Status Status { get; set; }

        public void AddCreateInformation(int createdByUserId) 
        {
            Status = Status.Active;
            CreatedByUserId = createdByUserId;
            DateCreated = DateTime.UtcNow.AddMonths(3);
        }

        public void AddUpdateInformation(int lastUpdatedByUserId) 
        {
            LastUpdatedByUserId = lastUpdatedByUserId;
            LastUpdated = DateTime.UtcNow.AddMonths(3);
        }

        public void AddDeleteInformation(int deletedByUserId) 
        {
            Status = Status.Deleted;
            DeletedByUserId = deletedByUserId;
            DateDeleted = DateTime.UtcNow.AddMonths(3);
        }

        public void AddHiddenInformation(int hiddenByUserId) 
        {
            Status = Status.Hidden;
            HiddenByUserId = hiddenByUserId;
            DateHidden = DateTime.UtcNow.AddMonths(3);
        }
    }

    public enum Status 
    {
        Active, 
        Hidden,
        Deleted
    }
}