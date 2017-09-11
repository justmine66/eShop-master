using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    /// <summary>
    /// 银行卡类型枚举
    /// </summary>
    public class CardType
        : Enumeration
    {
        /// <summary>
        /// 银联
        /// </summary>
        public static CardType unionPlay = new CardType(1, "unionPlay");
        /// <summary>
        /// 支付宝
        /// </summary>
        public static CardType aPlay = new CardType(2, "APlay");
        /// <summary>
        /// 微信
        /// </summary>
        public static CardType weChat = new CardType(3, "WeChat");
        /// <summary>
        /// 银行卡类型列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CardType> List() => new[] { unionPlay, aPlay, weChat };

        protected CardType() { }
        public CardType(int id, string name)
            : base(id, name) { }

        /// <summary>
        /// 根据名称获取银行卡类型
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>银行卡类型</returns>
        public static CardType FromName(string name)
        {
            var state = List().SingleOrDefault(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Possible values for CardTypes{string.Join(",", List())}");
            }

            return state;
        }
        /// <summary>
        /// 根据标识获取银行卡类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static CardType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Possible values for CardType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
