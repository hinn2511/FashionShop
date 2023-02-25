namespace API.DTOs.Request.ConfigurationRequest
{
    public class UpdateCustomerLoginBackgroundRequest 
    {
        public string ClientLoginBackground { get; set; }
        public int ClientLoginPhotoId { get; set; }
    }

    public class UpdateCustomerRegisterBackgroundRequest 
    {
        public string ClientRegisterBackground { get; set; }
        public int ClientRegisterPhotoId { get; set; }
    }

    public class UpdateAdministratorLoginBackgroundRequest 
    {
         public string AdministratorLoginBackground { get; set; }
        public int AdministratorLoginPhotoId { get; set; }
    }
}