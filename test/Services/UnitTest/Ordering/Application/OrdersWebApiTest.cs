using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.Ordering.Application
{
    using MediatR;
    using global::Ordering.API.Application.Queries;
    using global::Ordering.API.Application.Commands;
    using global::Ordering.API.Controllers;
    using global::Ordering.API.Infrastructure.Services;
    using Xunit;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;

    public class OrdersWebApiTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IOrderQueries> _orderQueriesMock;
        private readonly Mock<IIdentityService> _identityServiceMock;

        public OrdersWebApiTest()
        {
            this._mediatorMock = new Mock<IMediator>();
            this._orderQueriesMock = new Mock<IOrderQueries>();
            this._identityServiceMock = new Mock<IIdentityService>();
        }

        [Fact]
        public async Task Cancel_order_with_requestId_successAsync()
        {
            this._mediatorMock.Setup(m => m.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(true));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);
            OkResult response = await orderController.CancelOrderAsync(new CancelOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

            Assert.Equal(response.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Cancel_order_with_requestId_bad_requestAsync()
        {
            this._mediatorMock.Setup(m => m.Send(It.IsAny<IdentifiedCommand<CancelOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(false));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);
            BadRequestResult response = await orderController.CancelOrderAsync(new CancelOrderCommand(1), Guid.NewGuid().ToString()) as BadRequestResult;

            Assert.Equal(response.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Ship_order_with_requestId_successAsync()
        {
            this._mediatorMock.Setup(m => m.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(true));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);
            OkResult response = await orderController.ShipOrderAsync(new ShipOrderCommand(1), Guid.NewGuid().ToString()) as OkResult;

            Assert.Equal(response.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Ship_order_with_requestId_bad_requestAsync()
        {
            this._mediatorMock.Setup(m => m.Send(It.IsAny<IdentifiedCommand<ShipOrderCommand, bool>>(), default(CancellationToken)))
                .Returns(Task.FromResult(false));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);
            BadRequestResult response = await orderController.ShipOrderAsync(new ShipOrderCommand(1), Guid.NewGuid().ToString()) as BadRequestResult;

            Assert.Equal(response.StatusCode, (int)System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_order_success()
        {
            this._orderQueriesMock.Setup(o => o.GetOrderAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(new object()));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);

            OkObjectResult actionResult = await orderController.GetOrderAsync(123) as OkObjectResult;

            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_orders_success()
        {
            this._orderQueriesMock.Setup(o => o.GetOrdersAsync())
                .Returns(Task.FromResult(Enumerable.Empty<object>()));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);

            OkObjectResult actionResult = await orderController.GetOrdersAsync() as OkObjectResult;

            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_CardTypes_success()
        {
            this._orderQueriesMock.Setup(o => o.GetCardTypesAsync())
                .Returns(Task.FromResult(Enumerable.Empty<object>()));

            var orderController = new OrdersController(this._mediatorMock.Object, this._orderQueriesMock.Object, this._identityServiceMock.Object);

            OkObjectResult actionResult = await orderController.GetCardTypesAsync() as OkObjectResult;

            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }
    }
}
