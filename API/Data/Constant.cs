using System;
using System.Collections.Generic;
using System.IO;

namespace API.Data
{
    public class Constant
    {
        public static readonly Dictionary<string, string> ImageContentType = new()
        {
            { "jpg", "image/jpeg" },
            { "jpeg", "image/jpeg" },
            { "png", "image/png" }
        };

        public static readonly Dictionary<string, string> VideoContentType = new()
        {
            { "mp4", "video/mp4" },
        };

        public static string DownloadFileUrl = "https://localhost:5001/file/";

        public static readonly string UploadFolderPath = Path.Combine(Environment.CurrentDirectory, "UploadedFiles");
        public static readonly int DefaultBufferSize = 4096;
        public static readonly int DefaultImageHeight = 1000;
        public static readonly int DefaultImageWidth = 1500;
    }
}