using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Runtime.Serialization;
using Ordering.API.Application.Models;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 创建订单命令
    /// </summary>
    [DataContract]
    public class CreateOrderCommand
        :IRequest<bool>
    {
        [DataMember]
        private readonly List<OrderItemDTO> _orderItems;

        /// <summary>
        /// 用户标识
        /// </summary>
        [DataMember]
        public string UserId { get; private set; }
        
        /// <summary>
        /// 街道
        /// </summary>
        [DataMember]
        public string Street { get; private set; }

        /// <summary>
        /// 城市
        /// </summary>
        [DataMember]
        public string City { get; private set; }

        /// <summary>
        /// 省份
        /// </summary>
        [DataMember]
        public string State { get; private set; }

        /// <summary>
        /// 国家
        /// </summary>
        [DataMember]
        public string Country { get; private set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataMember]
        public string ZipCode { get; private set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        [DataMember]
        public string CardNumber { get; private set; }

        /// <summary>
        /// 银行卡持卡人姓名
        /// </summary>
        [DataMember]
        public string CardHolderName { get; private set; }

        /// <summary>
        /// 银行卡过期时间
        /// </summary>
        [DataMember]
        public DateTime CardExpiration { get; private set; }

        /// <summary>
        /// 支付密码
        /// </summary>
        [DataMember]
        public string CardSecurityNumber { get; private set; }

        /// <summary>
        /// 银行卡类型标识
        /// </summary>
        [DataMember]
        public int CardTypeId { get; private set; }

        /// <summary>
        /// 订单项集合
        /// </summary>
        [DataMember]
        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

        public CreateOrderCommand()
        {
            this._orderItems = new List<OrderItemDTO>();
        }

        /// <summary>
        /// 初始化一个创建订单命令
        /// </summary>
        /// <param name="basketItems">购物篮子商品集</param>
        /// <param name="userId">用户标识</param>
        /// <param name="city">城市</param>
        /// <param name="street">街道</param>
        /// <param name="state">省份</param>
        /// <param name="country">国家</param>
        /// <param name="zipcode">邮政编码</param>
        /// <param name="cardNumber">银行卡号</param>
        /// <param name="cardHolderName">银行卡持卡人姓名</param>
        /// <param name="cardExpiration">银行卡过期时间</param>
        /// <param name="cardSecurityNumber">支付密码</param>
        /// <param name="cardTypeId">银行卡类型标识</param>
        public CreateOrderCommand(List<BasketItem> basketItems, 
            string userId, string city, string street, string state, string country, string zipcode,
            string cardNumber, string cardHolderName, DateTime cardExpiration,
            string cardSecurityNumber, int cardTypeId) : this()
        {
            this._orderItems = this.MapToOrderItems(basketItems);
            this.UserId = userId;
            this.City = city;
            this.Street = street;
            this.State = state;
            this.Country = country;
            this.ZipCode = zipcode;
            this.CardNumber = cardNumber;
            this.CardHolderName = cardHolderName;
            this.CardExpiration = cardExpiration;
            this.CardSecurityNumber = cardSecurityNumber;
            this.CardTypeId = cardTypeId;
            this.CardExpiration = cardExpiration;
        }
        //购物篮子映射到订单项集合
        private List<OrderItemDTO> MapToOrderItems(List<BasketItem> basketItems)
        {
            var result = new List<OrderItemDTO>();
            basketItems.ForEach((item) => {
                result.Add(new OrderItemDTO()
                {
                    ProductId = int.TryParse(item.ProductId, out int id) ? id : -1,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl,
                    UnitPrice = item.UnitPrice,
                    Units = item.Quantity
                });
            });
            return result;
        }

        /// <summary>
        /// 订单项-数据传输对象
        /// </summary>
        public class OrderItemDTO
        {
            /// <summary>
            /// 商品Id
            /// </summary>
            public int ProductId { get; set; }
            /// <summary>
            /// 商品名称
            /// </summary>
            public string ProductName { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public decimal UnitPrice { get; set; }
            /// <summary>
            /// 折扣
            /// </summary>
            public decimal Discount { get; set; }
            /// <summary>
            /// 购买数量
            /// </summary>
            public int Units { get; set; }
            /// <summary>
            /// 图片Url
            /// </summary>
            public string PictureUrl { get; set; }
        }
    }
}
