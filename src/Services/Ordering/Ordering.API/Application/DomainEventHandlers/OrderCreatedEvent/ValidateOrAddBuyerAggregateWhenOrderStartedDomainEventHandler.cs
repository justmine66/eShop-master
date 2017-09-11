using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Infrastructure.Services;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers.OrderCreatedEvent
{
    /// <summary>
    /// 当处理订单创建领域事件时，验证或添加买家聚合。
    /// </summary>
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
        : IAsyncNotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IIdentityService _identityService;

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(
            ILoggerFactory logger, 
            IBuyerRepository buyerRepository, 
            IIdentityService identityService)
        {
            this._buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            this._identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(OrderCreatedDomainEvent @event)
        {
            var cardTypeId = (@event.CardTypeId != 0) ? @event.CardTypeId : 1;

            var buyer = await _buyerRepository.FindAsync(@event.UserId);
            bool buyerOriginallyExisted = (buyer == null) ? false : true;

            if (!buyerOriginallyExisted)
            {
                buyer = new Buyer(@event.UserId);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment Method on {DateTime.UtcNow}",
                                           @event.CardNumber,
                                           @event.CardSecurityNumber,
                                           @event.CardHolderName,
                                           @event.CardExpiration,
                                           @event.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ? _buyerRepository.Update(buyer) : _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveEntitiesAsync();

            _logger.CreateLogger(nameof(ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler)).LogTrace($"Buyer {buyerUpdated.Id} and related payment method were validated or updated for orderId: {@event.Order.Id}.");
        }
    }
}
