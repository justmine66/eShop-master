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
    /// 买家聚合根
    /// </summary>
    public class Buyer
        : Entity, IAggregateRoot
    {
        #region [ 字段和构造函数重载 ]

        //付款方式列表
        private List<PaymentMethod> _paymentMethods;

        protected Buyer()
        {
            this._paymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string identityGuid) : this()
        {
            this.IdentityGuid = !string.IsNullOrWhiteSpace(identityGuid)
                ? identityGuid : throw new ArgumentNullException(nameof(identityGuid));
        }

        #endregion

        #region [ 公共属性 ]

        /// <summary>
        /// 身份标识
        /// </summary>
        public string IdentityGuid { get; private set; }

        /// <summary>
        /// 付款方式列表
        /// </summary>
        public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

        #endregion

        #region [ 发布领域事件的命令方法 ]

        /// <summary>
        /// 验证或添加付款方式
        /// </summary>
        /// <param name="cardTypeId">银行卡类型标识</param>
        /// <param name="alias">别名</param>
        /// <param name="cardNumber">卡号</param>
        /// <param name="securityNumber">密码</param>
        /// <param name="cardHolderName">持卡人姓名</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="orderId">订单标识</param>
        /// <returns></returns>
        public PaymentMethod VerifyOrAddPaymentMethod(
            int cardTypeId, string alias, string cardNumber,
            string securityNumber, string cardHolderName, DateTime expiration,
            int orderId)
        {
            //判断该支付方式是否存在
            var existingPayment = this._paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

            if (existingPayment != null)//已存在
            {
                //添加领域事件
                this.AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingPayment;
            }
            else
            {
                //新增一个支付方式
                var paymentMethod = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);
                this._paymentMethods.Add(paymentMethod);

                //添加领域事件
                this.AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, paymentMethod, orderId));

                return paymentMethod;
            }
        }

        #endregion
    }
}
