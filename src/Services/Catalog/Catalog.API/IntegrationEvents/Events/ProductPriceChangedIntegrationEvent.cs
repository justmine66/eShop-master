using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    /// <summary>
    /// 产品价格已改变事件
    /// </summary>
    public class ProductPriceChangedIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// 产品标识
        /// </summary>
        public int ProductId { get; private set; }

        /// <summary>
        /// 新价格
        /// </summary>
        public decimal NewPrice { get; private set; }

        /// <summary>
        /// 旧价格
        /// </summary>

        public decimal OldPrice { get; private set; }

        /// <summary>
        /// 初始化产品价格已改变事件实例
        /// </summary>
        /// <param name="productId">产品标识</param>
        /// <param name="newPrice">新价格</param>
        /// <param name="oldPrice">旧价格</param>
        public ProductPriceChangedIntegrationEvent(int productId, decimal newPrice, decimal oldPrice)
        {
            ProductId = productId;
            NewPrice = newPrice;
            OldPrice = oldPrice;
        }

    }
}
