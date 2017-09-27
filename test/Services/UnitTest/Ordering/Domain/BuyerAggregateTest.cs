using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Exceptions;

namespace UnitTest.Ordering.Domain
{
    /// <summary>
    /// 买家聚合根单元测试
    /// </summary>
    public class BuyerAggregateTest
    {
        public BuyerAggregateTest() { }

        [Fact]
        public void Create_buyer_success()
        {
            string identity = Guid.NewGuid().ToString();

            var fakeBuyer = new Buyer(identity);

            Assert.NotNull(fakeBuyer);
        }

        [Fact]
        public void Create_buyer_fail()
        {
            string identity = string.Empty;

            Assert.Throws<ArgumentNullException>(() => { new Buyer(identity); });
        }

        [Fact]
        public void VerifyOrAdd_buyer_payment_method_success()
        {
            string identity = Guid.NewGuid().ToString();
            var fakeBuyer = new Buyer(identity);

            var result = fakeBuyer.VerifyOrAddPaymentMethod(
                cardTypeId: 2,
                alias: "支付宝",
                cardNumber: "60021042300156",
                securityNumber: "456358",
                cardHolderName: "用于测试的持卡人",
                expiration: DateTime.UtcNow.AddDays(1),
                orderId: 1);

            Assert.NotNull(result);
        }

        [Fact]
        public void Create_payment_method_success()
        {
            var fakePaymentMethod = new PaymentMethod(
                cardTypeId: 1,
                alias: "银联",
                cardNumber: "456137979813456",
                securityNumber: "789654",
                cardHolderName: "张三",
                expiration: DateTime.UtcNow.AddDays(1));

            Assert.NotNull(fakePaymentMethod);
        }

        [Fact]
        public void Create_payment_method_fail()
        {
            Assert.Throws<OrderingDomainException>(() =>
            {
                var fakePaymentMethod = new PaymentMethod(
                cardTypeId: 1,
                alias: "银联",
                cardNumber: "456137979813456",
                securityNumber: "789654",
                cardHolderName: "张三",
                expiration: DateTime.UtcNow.AddDays(-1));
            }); ;
        }

        [Fact]
        public void Payment_method_is_equal_to()
        {
            int cardTypeId = 1;
            string cardNumber = "456137979813456";
            DateTime expiration = DateTime.UtcNow.AddDays(1);

            var fakePaymentMethod = new PaymentMethod(
                cardTypeId: cardTypeId,
                alias: "银联",
                cardNumber: cardNumber,
                securityNumber: "789654",
                cardHolderName: "张三",
                expiration: expiration);

            var equal = fakePaymentMethod.IsEqualTo(cardTypeId, cardNumber, expiration);
            var notEqual = fakePaymentMethod.IsEqualTo(cardTypeId, "", expiration);
            Assert.True(equal);
            Assert.False(notEqual);
        }

        [Fact]
        public void Add_new_payment_method_raise_new_event()
        {
            string identity = Guid.NewGuid().ToString();
            var fakeBuyer = new Buyer(identity);

            fakeBuyer.VerifyOrAddPaymentMethod(
                cardTypeId: 2,
                alias: "支付宝",
                cardNumber: "60021042300156",
                securityNumber: "456358",
                cardHolderName: "用于测试的持卡人",
                expiration: DateTime.UtcNow.AddDays(1),
                orderId: 1);

            Assert.Equal<int>(fakeBuyer.DomainEvents.Count, 1);
        }
    }
}
