using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Filters
{
    public class JosnErrorResponse
    {
        public string[] Messages { get; set; }
        public object DeveloperMessage { get; set; }
    }
}
