namespace API.DTOs.Response.ConfigurationResponse
{
    public class SettingsResponse
    {
        public string ClientLoginBackground { get; set; }
        public string ClientRegisterBackground { get; set; }
        public string AdministratorLoginBackground { get; set; }
    }

    public class ClientSettingsResponse : SettingsResponse
    {
    }
}