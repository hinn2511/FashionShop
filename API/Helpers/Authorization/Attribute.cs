using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAttribute : Attribute
    {
        public RoleAttribute(string normalizeRoleName)
        {
            NormalizeRoleName = normalizeRoleName.ToUpper();
        }

        public string NormalizeRoleName { get; set; }
    }
}