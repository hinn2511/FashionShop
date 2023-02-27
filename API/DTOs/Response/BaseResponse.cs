using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static API.Data.Constant;

namespace API.DTOs.Response
{
    public class BaseResponse
    {
        public int Id { get; set; }
    }

    public class BaseResponseMessage
    {

        public BaseResponseMessage(bool isSuccess,HttpStatusCode httpStatusCode, string message)
        {
            this.IsSuccess = isSuccess;
            this.Status = httpStatusCode;
            this.Message = message ;
        }

        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class BasePhotoResponse
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public FileType FileType { get; set; }
    }
    
}