using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class AsyncExtensions
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, Task> function)
        {
            foreach (var e in list) 
            {
                await function(e);
            }
        }
    }
}