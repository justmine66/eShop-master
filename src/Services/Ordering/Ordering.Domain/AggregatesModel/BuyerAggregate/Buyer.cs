using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using Ordering.Domain.Events;
using System.Text;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    /// <summary>
    /// 买家实体
    /// </summary>
    public class Buyer
        : Entity, IAggregateRoot
    {
        public string IdentityGuid { get; private set; }

        private List<PaymentMethod> _paymentMethods;

        public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

        protected Buyer()
        {
            _paymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string identityGuid) : this()
        {
            this.IdentityGuid = !string.IsNullOrWhiteSpace(identityGuid) ? identityGuid : throw new OrderingDomainException(nameof(identityGuid));
        }

        public PaymentMethod VerifyOrAddPaymentMethod(
            int cardTypeId, string alias, string cardNumber,
            string securityNumber, string cardHolderName, DateTime expiration, int orderId)
        {
            var existingPayment = this._paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

            if (existingPayment != null)
            {
                this.AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingPayment;
            }
            else {
                var paymentMethod = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
                this._paymentMethods.Add(paymentMethod);

                this.AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this,paymentMethod,orderId));

                return paymentMethod;
            }
        }
    }
}
