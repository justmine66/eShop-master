using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.API.Infrastructure.Services;
using Ordering.Infrastructure.Idempotency;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderCommandIdentifiedHandler : IdentifierCommandHandler<CreateOrderCommand, bool>
    {
        public CreateOrderCommandIdentifiedHandler(IMediator mediator, IRequestManager requestManager) 
            : base(mediator, requestManager)
        {
        }

        protected override bool CreateResultForDuplicateRequest()
        {
            return true;       
        }
    }

    /// <summary>
    /// 创建订单命令处理
    /// </summary>
    public class CreateOrderCommandHandler
        : IAsyncRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIdentityService _identityService;
        private readonly IMediator _mediator;

        public CreateOrderCommandHandler(IMediator mediator, 
            IOrderRepository orderRepository, 
            IIdentityService identityService)
        {
            this._orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="message">订单创建命令</param>
        /// <returns>是否创建成果</returns>
        public async Task<bool> Handle(CreateOrderCommand message)
        {
            var address = new Address(message.Street, message.City, message.State, message.Country, message.ZipCode);
            var order = new Order(message.UserId, address, message.CardTypeId, message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration);

            foreach (var item in message.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            this._orderRepository.Add(order);

            return await this._orderRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
