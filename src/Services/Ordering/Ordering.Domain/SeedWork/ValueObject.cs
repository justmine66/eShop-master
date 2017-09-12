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
            if (object.ReferenceEquals(left, null) ^ object.ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
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
        /// 获取值对象下所有原子项组成的集合
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<object> GetAtomicValues();

        /// <summary>
        /// 等于,所有原子项值相等代表两个值对象相等.
        /// </summary>
        /// <param name="obj">值对象</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
                return false;

            var other = obj as ValueObject;
            IEnumerator<object> thisValues = this.GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();
            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                //001 如果同一位置存在原子值都为空的情况,则不相等.
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }
                //002 判断原子值是否相等
                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }
            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override int GetHashCode()
        {
            return this.GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public ValueObject GetCopy()
        {
            return this.MemberwiseClone() as ValueObject;
        }
    }
}
