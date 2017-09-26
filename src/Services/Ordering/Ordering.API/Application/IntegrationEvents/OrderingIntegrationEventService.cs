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
            //001 将事件保存到聚合根所在的数据库中，由于事件表和聚合根表同属一个数据库，整个过程只需要一个本地事务就能完成。
            await this.SaveEventAndOrderingContextChangesAsync(@event);
            //说明：保证发布事件和标记“已发布”之间原子性，需要采用分布式事务，但是违背了事件表的初衷；一种解决方法是将事件的消费方创建成幂等        的，即消费方可以多次消费同一个事件。
            //过程：事件发送和数据库更新采用各自的事务管理，此时有可能发生的情况是事件发送成功而数据库更新失败，这样在下一次事件发布操作中，由于       先前发布过的事件在数据库中依然是“未发布”状态，该事件将被重新发布到消息系统中，导致事件重复，但由于事件的消费方是幂等的，因      此事件重复不会存在问题。
            //002 发布事件
            this._eventBus.Publish(@event);
            //003 将表中的事件标记成“已发布”状态。
            await this._eventLogService.MarkEventAsPublishedAsync(@event);
        }

        private async Task SaveEventAndOrderingContextChangesAsync(IntegrationEvent @event)
        {
            //说明：不同微服务之间，要保证业务操作和事件发布之间的原子性，本来需采用分布式(XA)事务，这违背了微服务架构追求最终一致性的原则；
            //采用事件表的方式，通过一个本地事务，保证业务操作和事件发布之间的原子性；然后，在一个单独的任务中读取事件表中未发布的事件，再将事件发布到消息中间件中。 
            await ResilientTransaction.New(this._orderingContext)
                .ExecuteAsync(async () =>
           {
               await this._orderingContext.SaveChangesAsync();
               await this._eventLogService.SaveEventAsync(@event, this._orderingContext.Database.CurrentTransaction.GetDbTransaction());
           });
        }
    }
}
