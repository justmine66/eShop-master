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

        [DataMember]
        public string UserId { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string Street { get; private set; }

        [DataMember]
        public string State { get; private set; }

        [DataMember]
        public string Country { get; private set; }

        [DataMember]
        public string ZipCode { get; private set; }

        [DataMember]
        public string CardNumber { get; private set; }

        [DataMember]
        public string CardHolderName { get; private set; }

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string CardSecurityNumber { get; private set; }

        [DataMember]
        public int CardTypeId { get; private set; }

        [DataMember]
        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

        public CreateOrderCommand()
        {
            this._orderItems = new List<OrderItemDTO>();
        }

        public CreateOrderCommand(List<BasketItem> basketItems, string userId, string city, string street, string state, string country, string zipcode,
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

        public class OrderItemDTO
        {
            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public decimal UnitPrice { get; set; }

            public decimal Discount { get; set; }

            public int Units { get; set; }

            public string PictureUrl { get; set; }
        }
    }
}
