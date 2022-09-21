using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.OtherModel
{
    public class UploadedFile : BaseEntity
    {
        public string ContentType { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
    }
}