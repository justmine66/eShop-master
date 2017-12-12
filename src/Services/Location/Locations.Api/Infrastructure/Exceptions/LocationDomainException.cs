using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Locations.Api.Infrastructure.Exceptions
{
    public class LocationDomainException : Exception
    {
        public LocationDomainException()
        {

        }

        public LocationDomainException(string message)
            : base(message)
        {

        }
        public LocationDomainException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
