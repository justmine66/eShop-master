using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure.Idempotency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    public class ShipOrderCommandIdentifiedHandler : IdentifierCommandHandler<ShipOrderCommand, bool>
    {
        public ShipOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;                
        }
    }
    /// <summary>
    /// 派送订单命令处理
    /// </summary>
    public class ShipOrderCommandHandler : IAsyncRequestHandler<ShipOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;

        public ShipOrderCommandHandler(IOrderRepository orderRepository)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<bool> Handle(ShipOrderCommand message)
        {
            var orderToUpdate = await _orderRepository.GetAsync(message.OrderNumber);
            orderToUpdate.SetShippedStatus();
            return await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
