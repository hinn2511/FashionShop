using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Authorization
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int RefreshTokenTTL { get; set; }
    }
}