using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    /// <summary>
    /// 购物车结账
    /// </summary>
    public class BasketCheckout
    {
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 街道
        /// </summary>

        public string Street { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public string State { get; set; }

        public string Country { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>

        public string ZipCode { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>

        public string CardNumber { get; set; }

        /// <summary>
        /// 持卡人姓名
        /// </summary>

        public string CardHolderName { get; set; }

        /// <summary>
        /// 银行卡过期时间
        /// </summary>

        public DateTime CardExpiration { get; set; }

        /// <summary>
        /// 支付密码
        /// </summary>

        public string CardSecurityNumber { get; set; }

        /// <summary>
        /// 银行卡类型
        /// </summary>

        public int CardTypeId { get; set; }

        /// <summary>
        /// 买家
        /// </summary>

        public string Buyer { get; set; }

        /// <summary>
        /// 请求标识
        /// </summary>

        public Guid RequestId { get; set; }
    }
}
