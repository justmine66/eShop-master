using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Infrastructure;
using EventBus.Abstractions;
using EventBus.Events;
using IntegrationEventLogEF.Services;
using IntegrationEventLogEF.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.API.IntegrationEvents
{
    /// <summary>
    /// 目录一体化事件服务
    /// </summary>
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService
    {
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IEventBus _eventBus;
        private readonly CatalogContext _catalogContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public CatalogIntegrationEventService(IEventBus eventBus, CatalogContext catalogContext,
        Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _eventLogService = _integrationEventLogServiceFactory(_catalogContext.Database.GetDbConnection());
        }

        /// <summary>
        /// 通过事件总线发布事件
        /// </summary>
        /// <param name="evt">事件</param>
        /// <returns></returns>
        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            //001.发布事件
            _eventBus.Publish(evt);
            //002.标志事件为已发布
            await _eventLogService.MarkEventAsPublishedAsync(evt);
        }

        /// <summary>
        /// 保存事件并改变目录上下文的异步任务
        /// </summary>
        /// <param name="">事件</param>
        /// <returns></returns>
        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
            await ResilientTransaction.New(_catalogContext)
                .ExecuteAsync(async ()=> {
                    // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                    await _catalogContext.SaveChangesAsync();
                    await _eventLogService.SaveEventAsync(evt,
                        _catalogContext.Database.CurrentTransaction.GetDbTransaction());
                });
        }
    }
}
