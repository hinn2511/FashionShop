using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities.Other
{
    public class Photo : BaseEntity
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
    }
}