using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Models
{
    /// <summary>
    /// 客户购物车
    /// </summary>
    public class CustomerBasket
    {
        public string BuyerId { get; set; }
        public List<BasketItem> Items { get; set; }

        public CustomerBasket(string customerId)
        {
            this.BuyerId = customerId;
            this.Items = new List<BasketItem>();
        }
    }
}
