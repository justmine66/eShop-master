using Ordering.Domain.AggregatesModel.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Ordering
{
    public class AddressBuilder
    {
        public Address Build()
        {
            return new Address("凤翔街", "秀山", "重庆", "中国", "409900");
        }
    }
}
