using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// 枚举
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        /// <summary>
        /// 值
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        protected Enumeration() { }

        /// <summary>
        /// 初始化一个枚举实例
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="name">名称</param>
        protected Enumeration(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// 获取枚举实例中所有枚举字段表示的集合。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
        {
            var type = typeof(T);
            var fields = type.GetRuntimeFields();

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;//枚举字段
                if (locatedValue != null)
                {
                    yield return locatedValue;
                }
            }
        }

        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
            {
                return false;
            }

            var typeMatches = this.GetType().Equals(obj.GetType());//类型匹配
            var valueMatches = this.Id.Equals(otherValue.Id);//值匹配

            return typeMatches && valueMatches;
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// 获取两个枚举的绝对差额
        /// </summary>
        /// <param name="firstValue">枚举一</param>
        /// <param name="secondValue">枚举二</param>
        /// <returns>绝对差额</returns>
        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        /// <summary>
        /// 根据值获取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromValue<T>(int value)
            where T : Enumeration, new()
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        /// <summary>
        /// 根据名称获取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static T FromName<T>(string name)
            where T : Enumeration, new()
        {
            var matchingItem = Parse<T, string>(name, "name", item => item.Name == name);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate)
            where T : Enumeration, new()
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);//获取指定枚举字段

            if (matchingItem == null)
            {
                var message = $"'{value}' is not valid {description} in {typeof(T)}";

                throw new InvalidOperationException(message);
            }

            return matchingItem;
        }

        /// <summary>
        /// 比较枚举值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>1表示大于，0表示等于，-1表示小于</returns>
        public int CompareTo(object obj)
        {
            var other = obj as Enumeration;
            return other != null ? this.Id.CompareTo(other.Id) : -1;
        }
    }
}
