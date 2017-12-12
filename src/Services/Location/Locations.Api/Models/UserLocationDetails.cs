using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.Api.Models
{
    public class UserLocationDetails
    {
        public int LocationId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
