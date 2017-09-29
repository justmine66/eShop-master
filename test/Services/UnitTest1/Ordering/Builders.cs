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

    public class OrderBuilder
    {
        private readonly Order order;

        public OrderBuilder(Address address)
        {
            this.order = new Order(
                 userId: "userId" + new Random().Next(1, 1000),
                 address: address,
                 cardTypeId: 2,
                 cardNumber: "60021042300156",
                 cardSecurityNumber: new Random().Next(100000, 999999).ToString(),
                 cardHolderName: "张三",
                 cardExpiration: DateTime.UtcNow.AddDays(1),
                 buyerId: 1,
                 paymentMethodId: 1);
        }

        public OrderBuilder AddOrderItem(
            int productId,
            string productName,
            decimal unitPrice,
            decimal discount,
            string pictureUrl,
            int units = 1)
        {
            this.order.AddOrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            return this;
        }

        public Order Build()
        {
            return this.order;
        }
    }
}
