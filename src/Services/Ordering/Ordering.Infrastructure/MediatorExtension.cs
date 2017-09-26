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
    /// Mediator 扩展类
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
            //获取订单子域所有的领域实体
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());
            //合并所有领域实体的领域事件
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();
            //清空所有实体的领域事件列表(非常重要)
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
