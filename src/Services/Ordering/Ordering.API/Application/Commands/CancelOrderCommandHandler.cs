using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Ordering.Infrastructure.Idempotency;
using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.API.Application.Commands
{
    public class CancelOrderCommandIdentifiedHandler : IdentifierCommandHandler<CancelOrderCommand, bool>
    {
        public CancelOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager)
            : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;
        }
    }

    public class CancelOrderCommandHandler : IAsyncRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        public CancelOrderCommandHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> Handle(CancelOrderCommand message)
        {
            var orderToUpdate = await this._orderRepository.GetAsync(message.OrderNumber);
            orderToUpdate.SetCancelledStatus();
            return await this._orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
