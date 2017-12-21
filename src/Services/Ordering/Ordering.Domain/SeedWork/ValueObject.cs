using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// 值对象
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// 相等
        /// </summary>
        /// <param name="left">值对象,派生自ValueObject</param>
        /// <param name="right">值对象,派生自ValueObject</param>
        /// <returns>是否相等</returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }
            //到这里要么两个都为Null，要么都不为Null
            return left is null || left.Equals(right);
        }

        /// <summary>
        /// 不相等
        /// </summary>
        /// <param name="left">值对象,派生自ValueObject</param>
        /// <param name="right">值对象,派生自ValueObject</param>
        /// <returns>是否相等</returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        /// <summary>
        /// 在派生类中重写时，返回能够表示身份相等的所有组件。
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">其他值对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj)) return true;
            if (object.ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;

            var other = obj as ValueObject;
            return other.GetEqualityComponents().SequenceEqual(this.GetEqualityComponents());
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// 获取浅副本
        /// </summary>
        /// <returns></returns>
        public ValueObject GetCopy()
        {
            return this.MemberwiseClone() as ValueObject;
        }
    }
}
