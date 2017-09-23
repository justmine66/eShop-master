using EventBus.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.Commands;
using Ordering.API.Application.IntegrationEvents.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents.EventHandlings
{
    /// <summary>
    /// 用户检查接受集成事件
    /// </summary>
    public class UserCheckoutAcceptedIntegrationEventHandler : IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILoggerFactory _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public UserCheckoutAcceptedIntegrationEventHandler(IMediator mediator,
            ILoggerFactory logger, IOrderingIntegrationEventService orderingIntegrationEventService)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        }

        /// <summary>
        /// 启动创建订单流程的集成事件处理
        /// </summary>
        /// <param name="eventMsg">
        /// Integration event message which is sent by the
        /// basket.api once it has successfully process the 
        /// order items.
        /// </param>
        /// <returns></returns>
        public async Task Handle(UserCheckoutAcceptedIntegrationEvent eventMsg)
        {
            var result = false;

            // Send Integration event to clean basket once basket is converted to Order and before starting with the order creation process
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(eventMsg.UserId);
            await _orderingIntegrationEventService.PublishThroughEventBusAsync(orderStartedIntegrationEvent);

            if (eventMsg.RequestId != Guid.Empty)
            {
                var createOrderCommand = new CreateOrderCommand(eventMsg.Basket.Items, eventMsg.UserId, eventMsg.City, eventMsg.Street,
                    eventMsg.State, eventMsg.Country, eventMsg.ZipCode,
                    eventMsg.CardNumber, eventMsg.CardHolderName, eventMsg.CardExpiration,
                    eventMsg.CardSecurityNumber, eventMsg.CardTypeId);

                var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, eventMsg.RequestId);
                result = await _mediator.Send(requestCreateOrder);
            }

            _logger.CreateLogger(nameof(UserCheckoutAcceptedIntegrationEventHandler))
                .LogTrace(result ? $"UserCheckoutAccepted integration event has been received and a create new order process is started with requestId: {eventMsg.RequestId}" :
                    $"UserCheckoutAccepted integration event has been received but a new order process has failed with requestId: {eventMsg.RequestId}");
        }
    }
}
