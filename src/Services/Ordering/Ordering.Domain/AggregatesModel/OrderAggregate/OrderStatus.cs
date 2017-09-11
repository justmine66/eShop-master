using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 订单状态枚举
    /// </summary>
    public class OrderStatus
        : Enumeration
    {
        /// <summary>
        /// 已提交
        /// </summary>
        public static OrderStatus Submitted =
            new OrderStatus(1, nameof(Submitted).ToLowerInvariant());
        /// <summary>
        /// 等待验证
        /// </summary>
        public static OrderStatus AwaitingValidation =
            new OrderStatus(2, nameof(AwaitingValidation).ToLowerInvariant());
        /// <summary>
        /// 已确认库存
        /// </summary>
        public static OrderStatus StockConfirmed =
            new OrderStatus(3, nameof(StockConfirmed).ToLowerInvariant());
        /// <summary>
        /// 已支付
        /// </summary>
        public static OrderStatus Paid =
            new OrderStatus(4, nameof(Paid).ToLowerInvariant());
        /// <summary>
        /// 已派送
        /// </summary>
        public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToLowerInvariant());
        /// <summary>
        /// 取消
        /// </summary>
        public static OrderStatus Cancelled =
            new OrderStatus(6, nameof(Cancelled).ToLowerInvariant());

        protected OrderStatus() { }
        /// <summary>
        /// 初始化一个订单状态枚举实例
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="name">名称</param>
        protected OrderStatus(int id, string name) : base(id, name) { }

        /// <summary>
        /// 订单状态枚举列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<OrderStatus> List() =>
            new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Cancelled };

        /// <summary>
        /// 根据名称获取订单状态
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus:{string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        /// <summary>
        /// 根据标识获取订单状态
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns></returns>
        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
