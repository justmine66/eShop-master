using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Domain.Exceptions;
using Ordering.Domain.Events;

namespace UnitTest.Ordering.Domain
{
    /// <summary>
    /// 订单聚合根单元测试
    /// </summary>
    public class OrderAggregateTest
    {
        //成功创建订单项
        [Fact]
        public void Create_order_item_success()
        {
            var fakeOrderItem = new OrderItem(
                productId: 1,
                productName: "FakeProductName",
                unitPrice: 23.22m,
                discount: 10.2m,
                PictureUrl: "FakePictureUrl",
                units: 2);

            Assert.NotNull(fakeOrderItem);
        }
        //有效购买数量
        [Fact]
        public void Invalid_numbers_of_units()
        {
            Assert.Throws<OrderingDomainException>(() =>
            {
                var fakeOrderItem = new OrderItem(
                productId: 1,
                productName: "FakeProductName",
                unitPrice: 23.22m,
                discount: 100.2m,
                PictureUrl: "FakePictureUrl",
                units: -1);
            });
        }
        //订单总额低于应用的折扣额
        [Fact]
        public void Invalid_total_of_order_item_lower_than_discount_apllied()
        {
            Assert.Throws<OrderingDomainException>(() =>
            {
                var fakeOrderItem = new OrderItem(
                productId: 1,
                productName: "FakeProductName",
                unitPrice: 23.22m,
                discount: 100.2m,
                PictureUrl: "FakePictureUrl",
                units: 1);
            });
        }
        //设置折扣额
        [Fact]
        public void discount_setting()
        {
            var fakeOrderItem = new OrderItem(
                productId: 1,
                productName: "FakeProductName",
                unitPrice: 23.22m,
                discount: 100.2m,
                PictureUrl: "FakePictureUrl",
                units: 10);

            Assert.Throws<OrderingDomainException>(() => { fakeOrderItem.SetNewDiscount(-1); });

            fakeOrderItem.SetNewDiscount(10m);
            Assert.Equal<decimal>(fakeOrderItem.GetCurrentDiscount(), 10m);
        }
        //设置图片路径
        [Fact]
        public void pictureUri_setting()
        {
            var fakeOrderItem = new OrderItem(
                productId: 1,
                productName: "FakeProductName",
                unitPrice: 23.22m,
                discount: 100.2m,
                PictureUrl: "FakePictureUrl",
                units: 10);

            fakeOrderItem.SetPictureUri("FakePictureUrl001");
        }
        //设置购买数量
        [Fact]
        public void Units_setting()
        {
            var fakeOrderItem = new OrderItem(
                productId: 1,
                productName: "FakeProductName",
                unitPrice: 23.22m,
                discount: 100.2m,
                PictureUrl: "FakePictureUrl",
                units: 10);

            Assert.Throws<OrderingDomainException>(() => { fakeOrderItem.AddUnits(-1); });

            fakeOrderItem.AddUnits(10);
            Assert.Equal<int>(fakeOrderItem.GetUnits(), 20);
        }

        [Fact]
        public void When_add_two_times_on_the_same_order_then_the_total_of_order_should_be_the_sum_of_two_items()
        {
            var address = new AddressBuilder().Build();
            var order = new OrderBuilder(address)
                .AddOrderItem(1, "fakeProductName001", 12.2m, 1m, "fakePictureUrl001", 2)
                .AddOrderItem(1, "fakeProductName002", 12.2m, 1m, "fakePictureUrl002", 2)
                .Build();

            Assert.Equal<decimal>(48.8m, order.GetTotal());
        }

        [Fact]
        public void Add_new_order_raise_new_event()
        {
            var address = new AddressBuilder().Build();
            var fakeOrder = new Order(
                userId: "fakeUserId",
                address: address,
                cardTypeId: 1,
                cardNumber: "fakeCardNumber",
                cardSecurityNumber: "fakeCardSecurityNumber",
                cardHolderName: "fakeCardHolderName",
                cardExpiration: DateTime.UtcNow.AddDays(1),
                buyerId: 1,
                paymentMethodId: 2);

            Assert.Equal<int>(fakeOrder.DomainEvents.Count, 1);
        }

        [Fact]
        public void Add_order_and_domain_event_raise_new_event()
        {
            var address = new AddressBuilder().Build();
            var fakeOrder = new Order(
                userId: "fakeUserId",
                address: address,
                cardTypeId: 1,
                cardNumber: "fakeCardNumber",
                cardSecurityNumber: "fakeCardSecurityNumber",
                cardHolderName: "fakeCardHolderName",
                cardExpiration: DateTime.UtcNow.AddDays(1),
                buyerId: 1,
                paymentMethodId: 2);

            fakeOrder.AddDomainEvent(new OrderCreatedDomainEvent(fakeOrder,
                userId: "fakeUserId",
                cardTypeId: 1,
                cardNumber: "fakeCardNumber",
                cardSecurityNumber: "fakeCardSecurityNumber",
                cardHolderName: "fakeCardHolderName",
                cardExpiration: DateTime.UtcNow.AddDays(1)));

            Assert.Equal<int>(fakeOrder.DomainEvents.Count, 2);
        }

        [Fact]
        public void Remove_event_order_explicitly()
        {
            var address = new AddressBuilder().Build();
            var fakeOrder = new Order(
                userId: "fakeUserId",
                address: address,
                cardTypeId: 1,
                cardNumber: "fakeCardNumber",
                cardSecurityNumber: "fakeCardSecurityNumber",
                cardHolderName: "fakeCardHolderName",
                cardExpiration: DateTime.UtcNow.AddDays(1),
                buyerId: 1,
                paymentMethodId: 2);

            var domainEvent = new OrderCreatedDomainEvent(fakeOrder,
                userId: "fakeUserId",
                cardTypeId: 1,
                cardNumber: "fakeCardNumber",
                cardSecurityNumber: "fakeCardSecurityNumber",
                cardHolderName: "fakeCardHolderName",
                cardExpiration: DateTime.UtcNow.AddDays(1));
            fakeOrder.AddDomainEvent(domainEvent);
            fakeOrder.RemoveDomainEvent(domainEvent);

            Assert.Equal<int>(fakeOrder.DomainEvents.Count, 1);
        }
    }
}
