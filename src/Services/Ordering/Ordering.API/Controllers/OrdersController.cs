using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Ordering.API.Infrastructure.Services;
using Ordering.API.Application.Queries;
using Ordering.API.Application.Commands;

namespace Ordering.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IOrderQueries _orderQueries;
        private readonly IIdentityService _identityService;

        public OrdersController(IMediator mediator, IOrderQueries orderQueries, IIdentityService identityService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        // GET api/v1/Orders/{orderId:int}
        [Route("{orderId:int}")]
        [HttpGet]
        public async Task<IActionResult> GetOrderAsync(int orderId)
        {
            try
            {
                var order =await this._orderQueries
                    .GetOrderAsync(orderId);

                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET api/v1/Orders
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync(int orderId)
        {
            try
            {
                var orders = await this._orderQueries
                    .GetOrdersAsync();

                return Ok(orders);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET api/v1/orders/cardtypes
        [Route("cardtypes")]
        [HttpGet]
        public async Task<IActionResult> GetCardTypesAsync()
        {
            var cardTypes = await _orderQueries
                .GetCardTypesAsync();

            return Ok(cardTypes);
        }

        // PUT api/v1/orders/cancel
        [Route("cancel")]
        [HttpPut]//PUT操作是幂等的,即指不管进行多少次操作,结果都一样.
        public async Task<IActionResult> CancelOrderAsync([FromBody]CancelOrderCommand command,
            [FromHeader(Name = "x-requestid")]string requestId)
        {
            bool commandResult = false;
            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, guid);
                commandResult = await this._mediator.Send<bool>(requestCancelOrder);
            }

            return commandResult ? Ok() as IActionResult : BadRequest() as IActionResult;
        }

        // PUT api/v1/orders/ship
        [Route("ship")]
        [HttpPut]//PUT操作是幂等的,即指不管进行多少次操作,结果都一样.
        public async Task<IActionResult> ShipOrderAsync([FromBody]ShipOrderCommand command,
            [FromHeader(Name = "x-requestid")]string requestId)
        {
            bool commandResult = false;
            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var requestCancelOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, guid);
                commandResult = await this._mediator.Send<bool>(requestCancelOrder);
            }

            return commandResult ? Ok() as IActionResult : BadRequest() as IActionResult;
        }
    }
}
