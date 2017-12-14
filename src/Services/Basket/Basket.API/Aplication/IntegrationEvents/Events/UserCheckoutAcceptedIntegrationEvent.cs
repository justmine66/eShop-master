﻿using Basket.API.Models;
using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Aplication.IntegrationEvents.Events
{
    /// <summary>
    /// 用户已结账集成事件
    /// </summary>
    public class UserCheckoutAcceptedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; }

        public int OrderNumber { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public DateTime CardExpiration { get; set; }

        public string CardSecurityNumber { get; set; }

        public int CardTypeId { get; set; }

        public string Buyer { get; set; }

        public Guid RequestId { get; set; }

        public CustomerBasket Basket { get; }

        public UserCheckoutAcceptedIntegrationEvent(string userId, string city, string street,
            string state, string country, string zipCode, string cardNumber, string cardHolderName,
            DateTime cardExpiration, string cardSecurityNumber, int cardTypeId, string buyer, Guid requestId,
            CustomerBasket basket)
        {
            this.UserId = userId;
            this.City = city;
            this.Street = street;
            this.State = state;
            this.Country = country;
            this.ZipCode = zipCode;
            this.CardNumber = cardNumber;
            this.CardHolderName = cardHolderName;
            this.CardExpiration = cardExpiration;
            this.CardSecurityNumber = cardSecurityNumber;
            this.CardTypeId = cardTypeId;
            this.Buyer = buyer;
            this.Basket = basket;
            this.RequestId = requestId;
        }
    }
}