using Ordering.Domain.Events;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 订单实体
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
        public Order(string userId,
            Address address,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardHolderName,
            DateTime cardExpiration,
            int? buyerId = null,
            int? paymentMethodId = null)
        {
            //1、记录订单基本信息
            this._orderItems = new List<OrderItem>();
            this._buyerId = buyerId;
            this._paymentMethodId = paymentMethodId;
            this._orderStatusId = OrderStatus.Submitted.Id;
            this.Address = address;
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
                .FirstOrDefault();

            if (existingOrderForProduct != null)
            {
                if (discount > existingOrderForProduct.GetCurrentDiscount())
                {
                    existingOrderForProduct.SetNewDiscount(discount);
                    existingOrderForProduct.AddUnits(units);
                }
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
        /// <param name="orderStockRejectedItems"></param>
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

        //状态改变异常
        private void StatusChangeException(OrderStatus orderStatusToChange)
        {
            throw new Exceptions.OrderingDomainException($"Not possible to change order status from {this.OrderStatus.Name} to {orderStatusToChange.Name}.");
        }
    }
}
