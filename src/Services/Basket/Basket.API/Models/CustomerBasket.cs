using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    /// <summary>
    /// 用户购物车
    /// </summary>
    public class CustomerBasket
    {
        public string BuyId { get; set; }

        public List<BasketItem> Items { get; set; }

        public CustomerBasket(string buyId, List<BasketItem> items)
        {
            this.BuyId = buyId;
            this.Items = items;
        }
    }
}
