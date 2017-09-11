using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    /// <summary>
    /// 付款方式实体
    /// </summary>
    public class PaymentMethod
        : Entity
    {
        private string _alias;
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        private int _cartTypeId;
        /// <summary>
        /// 银行卡类型
        /// </summary>
        public CardType CardType { get; private set; }

        protected PaymentMethod() { }
        /// <summary>
        /// 初始一个化付款方式实例
        /// </summary>
        /// <param name="cardTypeId">银行卡类型标识</param>
        /// <param name="alias">别名</param>
        /// <param name="cardNumber">卡号</param>
        /// <param name="securityNumber">密码</param>
        /// <param name="cardHolderName">持卡人姓名</param>
        /// <param name="expiration">过期时间</param>
        public PaymentMethod(
            int cardTypeId,
            string alias,
            string cardNumber,
            string securityNumber,
            string cardHolderName,
            DateTime expiration)
        {
            this._cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            this._securityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            this._cardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new OrderingDomainException(nameof(expiration));
            }

            this._alias = alias;
            this._cartTypeId = cardTypeId;
            this._expiration = expiration;
        }

        /// <summary>
        /// 判断付款方式是否相同
        /// </summary>
        /// <param name="cardTypeId">银行卡类型标识</param>
        /// <param name="cardNumber">银行卡号</param>
        /// <param name="expiration">过期时间</param>
        /// <returns></returns>
        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return this._cartTypeId == cardTypeId
                && this._cardNumber == cardNumber
                && this._expiration == expiration;
        }
    }
}
