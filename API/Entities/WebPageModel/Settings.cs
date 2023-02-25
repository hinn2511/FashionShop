
namespace API.Entities.WebPageModel
{
    public class Settings
    {
        public int Id { get; set; }
        public string ClientLoginBackground { get; set; }
        public int ClientLoginPhotoId { get; set; }
        public string ClientRegisterBackground { get; set; }
        public int ClientRegisterPhotoId { get; set; }
        public string AdministratorLoginBackground { get; set; }
        public int AdministratorLoginPhotoId { get; set; }
    }
}