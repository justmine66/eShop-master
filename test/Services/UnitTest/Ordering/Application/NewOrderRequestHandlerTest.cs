using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Ordering.Application
{
    using Moq;
    using MediatR;
    using global::Ordering.API.Application.Models;
    using Xunit;
    using global::Ordering.Domain.AggregatesModel.OrderAggregate;
    using global::Ordering.Domain.AggregatesModel.BuyerAggregate;
    using global::Ordering.API.Infrastructure.Services;
    using global::Ordering.API.Application.Commands;
    using static global::Ordering.API.Application.Commands.CreateOrderCommand;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// 新订单命令处理单元测试
    /// </summary>
    public class NewOrderRequestHandlerTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IMediator> _mediatorMock;

        public NewOrderRequestHandlerTest()
        {
            this._orderRepositoryMock = new Mock<IOrderRepository>();
            this._identityServiceMock = new Mock<IIdentityService>();
            this._mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Handler_returns_false_if_order_is_not_persisted()
        {
            var buyer = "1234";
            var fakeOrderCmd = this.FakeOrderRequestWithBuyer(new Dictionary<string, object>()
            {
                ["cardExpiration"] = DateTime.UtcNow.AddYears(1)
            });

            this._orderRepositoryMock
                .Setup(orderRepo => orderRepo.GetAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(this.FakeOrder()));
            this._orderRepositoryMock
                .Setup(orderRepo => orderRepo.UnitOfWork.SaveChangesAsync(CancellationToken.None))
                .Returns(Task.FromResult(1));
            this._identityServiceMock
                .Setup(ism => ism.GetUserIdentity())
                .Returns(buyer);

            var handler = new CreateOrderCommandHandler(
                this._mediatorMock.Object,
                this._orderRepositoryMock.Object,
                this._identityServiceMock.Object);
            var result = await handler.Handle(fakeOrderCmd);

            Assert.False(result);
        }

        private Buyer FakeBuyer()
        {
            return new Buyer(Guid.NewGuid().ToString());
        }
        private Order FakeOrder()
        {
            return new Order("1", new Address("street", "city", "state", "country", "zipcode"), 1, "12", "111", "fakeName", DateTime.Now.AddYears(1));
        }
        private CreateOrderCommand FakeOrderRequestWithBuyer(Dictionary<string, object> args = null)
        {
            return new CreateOrderCommand(
                new List<BasketItem>(),
                userId: args != null && args.ContainsKey("userId") ? (string)args["userId"] : null,
                city: args != null && args.ContainsKey("city") ? (string)args["city"] : null,
                street: args != null && args.ContainsKey("street") ? (string)args["street"] : null,
                state: args != null && args.ContainsKey("state") ? (string)args["state"] : null,
                country: args != null && args.ContainsKey("country") ? (string)args["country"] : null,
                zipcode: args != null && args.ContainsKey("zipcode") ? (string)args["zipcode"] : null,
                cardNumber: args != null && args.ContainsKey("cardNumber") ? (string)args["cardNumber"] : "1234",
                cardExpiration: args != null && args.ContainsKey("cardExpiration") ? (DateTime)args["cardExpiration"] : DateTime.MinValue,
                cardSecurityNumber: args != null && args.ContainsKey("cardSecurityNumber") ? (string)args["cardSecurityNumber"] : "123",
                cardHolderName: args != null && args.ContainsKey("cardHolderName") ? (string)args["cardHolderName"] : "XXX",
                cardTypeId: args != null && args.ContainsKey("cardTypeId") ? (int)args["cardTypeId"] : 0);
        }
    }
}
