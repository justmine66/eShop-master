using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 订单项实体
    /// </summary>
    public class OrderItem
        : Entity
    {
        private string _productName;
        private string _pictureUrl;
        private decimal _unitPrice;
        private decimal _discount;
        private int _units;

        public int ProductId { get; private set; }

        protected OrderItem() { }
        public OrderItem(int productId,
            string productName,
            decimal unitPrice,
            decimal discount,
            string PictureUrl,
            int units = 1)
        {
            if (units < 0)
            {
                throw new OrderingDomainException("Invalid numbers of units");
            }

            if ((unitPrice * units) < discount)
            {
                throw new OrderingDomainException("The total of order item is lower than applied discount");
            }

            this.ProductId = productId;

            this._productName = productName;
            this._unitPrice = unitPrice;
            this._discount = discount;
            this._units = units;
            this._pictureUrl = PictureUrl;
        }

        /// <summary>
        /// 设置图片路径
        /// </summary>
        /// <param name="pictureUri"></param>
        public void SetPictureUri(string pictureUri)
        {
            if (!String.IsNullOrWhiteSpace(pictureUri))
            {
                this._pictureUrl = pictureUri;
            }
        }

        /// <summary>
        /// 获取当前折扣
        /// </summary>
        /// <returns></returns>
        public decimal GetCurrentDiscount()
        {
            return this._discount;
        }

        /// <summary>
        /// 获取购买数量
        /// </summary>
        /// <returns></returns>
        public int GetUnits()
        {
            return this._units;
        }

        /// <summary>
        /// 获取订单项商品名称
        /// </summary>
        /// <returns></returns>
        public string GetOrderItemProductName() => _productName;

        /// <summary>
        /// 设置新折扣
        /// </summary>
        /// <param name="discount"></param>
        public void SetNewDiscount(decimal discount)
        {
            if (discount < 0)
            {
                throw new OrderingDomainException("Discount is not valid");
            }

            this._discount = discount;
        }

        /// <summary>
        /// 添加购买数量
        /// </summary>
        /// <param name="units"></param>
        public void AddUnits(int units)
        {
            if (units < 0)
            {
                throw new OrderingDomainException("Invalid units");
            }

            _units += units;
        }
    }
}
