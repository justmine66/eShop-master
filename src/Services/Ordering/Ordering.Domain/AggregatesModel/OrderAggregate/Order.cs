using Ordering.Domain.Events;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 订单聚合根
    /// </summary>
    public class Order
        : Entity, IAggregateRoot
    {
        //下单日期
        private DateTime _orderDate;

        //地址
        public Address Address { get; private set; }

        //买家
        private int? _buyerId;

        //订单状态
        public OrderStatus OrderStatus { get; private set; }
        private int _orderStatusId;

        //描述
        private string _description;

        //订单项列表
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => this._orderItems;

        //付款方式
        private int? _paymentMethodId;

        protected Order() { this._orderItems = new List<OrderItem>(); }
        /// <summary>
        /// 初始化一个订单实例
        /// </summary>
        /// <param name="userId">用户标识</param>
        /// <param name="address">送货地址</param>
        /// <param name="cardTypeId">银行卡类型标识</param>
        /// <param name="cardNumber">银行卡号</param>
        /// <param name="cardSecurityNumber">支付密码</param>
        /// <param name="cardHolderName">持卡人姓名</param>
        /// <param name="cardExpiration">银行卡过期时间</param>
        /// <param name="buyerId">买家标识</param>
        /// <param name="paymentMethodId">支付方式</param>
        public Order(string userId,
            Address address,
            int cardTypeId, string cardNumber, string cardSecurityNumber, string cardHolderName, DateTime cardExpiration,
            int? buyerId = null,
            int? paymentMethodId = null)
        {
            //1、记录订单基本信息
            this._orderItems = new List<OrderItem>();
            this._buyerId = buyerId;
            this._paymentMethodId = paymentMethodId;
            this._orderStatusId = OrderStatus.Submitted.Id;
            this.Address = address;
            this._orderDate = DateTime.UtcNow;
            //2、添加订单创建领域事件
            this.AddOrderCreatedDomainEvent(userId,
                cardTypeId,
                cardNumber,
                cardSecurityNumber,
                cardHolderName,
                cardExpiration);
        }

        /// <summary>
        /// 添加订单项
        /// </summary>
        public void AddOrderItem(int productId,
            string productName,
            decimal unitPrice,
            decimal discount,
            string pictureUrl,
            int units = 1)
        {
            var existingOrderForProduct = this._orderItems
                .Where(o => o.ProductId == productId)
                .FirstOrDefault();//根据商品标识找到存在的订单项

            if (existingOrderForProduct != null)//存在
            {
                if (discount > existingOrderForProduct.GetCurrentDiscount())//当前折扣 > 原始折扣
                {
                    existingOrderForProduct.SetNewDiscount(discount);
                }

                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
                this._orderItems.Add(orderItem);
            }
        }

        /// <summary>
        /// 设置支付方式
        /// </summary>
        /// <param name="id"></param>
        public void SetPaymentId(int id)
        {
            this._paymentMethodId = id;
        }

        /// <summary>
        /// 设置买家
        /// </summary>
        /// <param name="id"></param>
        public void SetBuyerId(int id)
        {
            this._buyerId = id;
        }

        /// <summary>
        /// 设置订单状态为等待验证
        /// </summary>
        public void SetAwaitingValidationStatus()
        {
            if (this._orderStatusId == OrderStatus.Cancelled.Id ||
                this._orderStatusId != OrderStatus.Submitted.Id)
            {
                this.StatusChangeException(OrderStatus.AwaitingValidation);
            }

            this.AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(this.Id, this._orderItems));

            this._orderStatusId = OrderStatus.AwaitingValidation.Id;
            this._description = "the current order status had been seted to 'AwaitingValidationStatus'";
        }

        /// <summary>
        /// 设置订单状态为已确认库存
        /// </summary>
        public void SetStockConfirmedStatus()
        {
            if (this._orderStatusId != OrderStatus.AwaitingValidation.Id)
            {
                this.StatusChangeException(OrderStatus.StockConfirmed);
            }

            this.AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(this.Id));

            this._orderStatusId = OrderStatus.StockConfirmed.Id;
            this._description = "All the items were confirmed with available stock.";
        }

        /// <summary>
        /// 设置订单状态为已支付
        /// </summary>
        public void SetPaidStatus()
        {
            if (this._orderStatusId != OrderStatus.StockConfirmed.Id)
            {
                this.StatusChangeException(OrderStatus.Paid);
            }

            this.AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(this.Id, this._orderItems));

            this._orderStatusId = OrderStatus.Paid.Id;
            this._description = "the payment was confirmed";
        }

        /// <summary>
        /// 设置订单状态为已派送
        /// </summary>
        public void SetShippedStatus()
        {
            if (this._orderStatusId != OrderStatus.Paid.Id)
            {
                this.StatusChangeException(OrderStatus.Shipped);
            }

            this._orderStatusId = OrderStatus.Shipped.Id;
            this._description = "the order was shipped.";
        }

        /// <summary>
        /// 设置订单状态为已取消
        /// </summary>
        public void SetCancelledStatus()
        {
            if (this._orderStatusId == OrderStatus.Paid.Id ||
                this._orderStatusId == OrderStatus.Shipped.Id)
            {
                this.StatusChangeException(OrderStatus.Cancelled);
            }

            this._orderStatusId = OrderStatus.Cancelled.Id;
            this._description = $"The order was cancelled.";
        }

        /// <summary>
        /// 设置订单状态为已取消，当库存不足时
        /// </summary>
        /// <param name="orderStockRejectedItems">库存不足的订单项集合</param>
        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (this._orderStatusId == OrderStatus.AwaitingValidation.Id)
            {
                this.StatusChangeException(OrderStatus.Cancelled);
            }

            this._orderStatusId = OrderStatus.Cancelled.Id;

            var itemsStockRejectedProductNames = this._orderItems
                .Where(o => orderStockRejectedItems.Contains(o.ProductId))
                .Select(p => p.GetOrderItemProductName());

            var itemsStockRejectedDescription = string.Join(",", itemsStockRejectedProductNames);
            this._description = $"The product items don't have stock:{itemsStockRejectedDescription}";
        }

        public decimal GetTotal()
        {
            return this.OrderItems.Sum(o => o.GetUnits() * o.GetUnitPrice());
        }

        //添加订单创建领域事件
        private void AddOrderCreatedDomainEvent(string userId,
            int cardTypeid,
            string cardNumber,
            string cardSecurityNumber,
            string cardHolderName,
            DateTime cardExpiration)
        {
            var orderCreatedDomainEvent = new OrderCreatedDomainEvent(this,
                userId,
                cardTypeid,
                cardNumber,
                cardSecurityNumber,
                cardHolderName,
                cardExpiration);

            this.AddDomainEvent(orderCreatedDomainEvent);
        }

        //订单状态改变异常抛出
        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new Exceptions.OrderingDomainException($"Not possible to change order status from {this.OrderStatus.Name} to {orderStatusToChange.Name}.");
        }
    }
}
