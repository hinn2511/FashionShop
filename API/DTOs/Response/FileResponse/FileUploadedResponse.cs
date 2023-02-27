namespace API.DTOs.Response.FileResponse
{
    public class FileUploadedResponse
    {
        public FileUploadedResponse(int id, string url)
        {
            Id = id;
            Url = url;
        }

        public int Id { get; set; }
        public string Url { get; set; }
    }
}