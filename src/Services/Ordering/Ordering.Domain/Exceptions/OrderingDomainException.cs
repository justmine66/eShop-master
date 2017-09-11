using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Exceptions
{
    /// <summary>
    /// 订单域异常类
    /// </summary>
    public class OrderingDomainException : Exception
    {
        public OrderingDomainException(){ }

        public OrderingDomainException(string message)
            : base(message)
        { }

        public OrderingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
