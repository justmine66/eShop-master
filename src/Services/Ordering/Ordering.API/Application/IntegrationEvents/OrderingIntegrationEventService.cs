using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Events;
using System.Data.Common;
using IntegrationEventLogEF.Services;
using EventBus.Abstractions;
using Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using IntegrationEventLogEF.Utilities;

namespace Ordering.API.Application.IntegrationEvents
{
    public class OrderingIntegrationEventService : IOrderingIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly OrderingContext _orderingContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public OrderingIntegrationEventService(
            IEventBus eventBus,
            OrderingContext orderingContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            this._eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            this._orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            this._integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            this._eventLogService = this._integrationEventLogServiceFactory(this._orderingContext.Database.GetDbConnection());
        }

        /// <summary>
        /// 通过事件总线发布集成事件，从而通知其他微服务。
        /// </summary>
        /// <param name="event">集成事件</param>
        /// <returns></returns>
        public async Task PublishThroughEventBusAsync(IntegrationEvent @event)
        {
            await this.SaveEventAndOrderingContextChangesAsync(@event);
            this._eventBus.Publish(@event);
            await this._eventLogService.MarkEventAsPublishedAsync(@event);
        }

        private async Task SaveEventAndOrderingContextChangesAsync(IntegrationEvent @event)
        {
            //通过显示的硬编码 _context.Database.BeginTransaction(),使用EF的事务弹性策略,跨多个DbContexts.
            await ResilientTransaction.New(this._orderingContext)
                .ExecuteAsync(async () =>
           {
               //通过一个本地事务，保证原始订单和事件日志的原子性
               await this._orderingContext.SaveChangesAsync();
               await this._eventLogService.SaveEventAsync(evt, this._orderingContext.Database.CurrentTransaction.GetDbTransaction());
           });
        }
    }
}
