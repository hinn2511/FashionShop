namespace API.DTOs.Response.FileResponse
{
    public class FileUploadedResponse
    {
        public FileUploadedResponse(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
}