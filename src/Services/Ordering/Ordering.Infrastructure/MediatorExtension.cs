using MediatR;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Ordering.Infrastructure
{
    /// <summary>
    /// Mediator扩展类
    /// </summary>
    static class MediatorExtension
    {
        /// <summary>
        /// 异步发布实体领域事件
        /// </summary>
        /// <param name="mediator">进程内通信的调度代理人</param>
        /// <param name="ctx">订单域上线文</param>
        /// <returns></returns>
        public static async Task DispatchDomainEventAsync(this IMediator mediator, OrderingContext ctx)
        {
            //获取领域实体集合
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());
            //提取所有领域事件集合
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();
            //清空实体的领域事件列表
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.DomainEvents.Clear());
            //发布所有领域事件
            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);
        }
    }
}
