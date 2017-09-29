using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Ordering.Application
{
    using Xunit;
    using global::Ordering.API.Application.Models;
    using global::Ordering.API.Application.Commands;
    using global::Ordering.Infrastructure.Idempotency;
    using MediatR;
    using Moq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 幂等命令处理单元测试
    /// </summary>
    public class IdentifierCommandHandlerTest
    {
        private readonly Mock<IRequestManager> _requestManager;
        private readonly Mock<IMediator> _mediator;

        public IdentifierCommandHandlerTest()
        {
            this._requestManager = new Mock<IRequestManager>();
            this._mediator = new Mock<IMediator>();
        }

        [Fact]
        public async Task Handler_sends_command_when_order_no_exists()
        {
            //构造命令
            var fakeGuid = Guid.NewGuid();
            var fakeOrderCmd = new IdentifiedCommand<CreateOrderCommand, bool>(this.FakeOrderRequest(), fakeGuid);

            //构造命令处理
            //模拟逻辑：订单不存在，成功创建订单。
            this._requestManager
                .Setup(x => x.ExistAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));
            this._mediator
                .Setup(x => x.Send(It.IsAny<IRequest<bool>>(), CancellationToken.None))
                .Returns(Task.FromResult(true));
            var handler = new IdentifierCommandHandler<CreateOrderCommand, bool>(this._mediator.Object, this._requestManager.Object);

            //执行命令
            var result = await handler.Handle(fakeOrderCmd);

            Assert.True(result);
            this._mediator.Verify(m => m.Send(It.IsAny<IRequest<bool>>(), CancellationToken.None), Times.Once());
        }

        [Fact]
        public async Task Handler_sends_no_command_when_order_exists()
        {
            //构造命令
            var fakeGuid = Guid.NewGuid();
            var fakeOrderCmd = new IdentifiedCommand<CreateOrderCommand, bool>(this.FakeOrderRequest(), fakeGuid);

            //构造命令处理
            //模拟逻辑：订单存在，不处理命令（保证命令消费的幂等性）。
            this._requestManager
                .Setup(x => x.ExistAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            this._mediator
                .Setup(x => x.Send(It.IsAny<IRequest<bool>>(), CancellationToken.None))
                .Returns(Task.FromResult(true));
            var handler = new IdentifierCommandHandler<CreateOrderCommand, bool>(this._mediator.Object, this._requestManager.Object);

            //执行命令
            var result = await handler.Handle(fakeOrderCmd);

            Assert.False(result);
            this._mediator.Verify(m => m.Send(It.IsAny<IRequest<bool>>(), CancellationToken.None), Times.Never());
        }

        private CreateOrderCommand FakeOrderRequest(Dictionary<string, object> args = null)
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
