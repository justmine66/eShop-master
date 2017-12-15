using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Exceptions
{
    public class BasketDomainException : Exception
    {
        public BasketDomainException()
            : base()
        {

        }

        public BasketDomainException(string message)
            : base(message)
        {

        }

        public BasketDomainException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
