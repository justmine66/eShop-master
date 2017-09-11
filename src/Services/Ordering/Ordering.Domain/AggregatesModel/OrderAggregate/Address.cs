using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// 地址
    /// </summary>
    public class Address
        : ValueObject
    {
        /// <summary>
        /// 街道
        /// </summary>
        public String Street { get; private set; }

        /// <summary>
        /// 城市
        /// </summary>
        public String City { get; private set; }

        /// <summary>
        /// 省份
        /// </summary>

        public String State { get; private set; }

        /// <summary>
        /// 国家
        /// </summary>

        public String Country { get; private set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public String ZipCode { get; private set; }

        private Address() { }
        public Address(string street, string city, string state, string country, string zipcode)
        {
            this.Street = street;
            this.City = city;
            this.State = state;
            this.Country = country;
            this.ZipCode = zipcode;
        }

        /// <summary>
        /// 获取原子值数据集
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Street;
            yield return this.City;
            yield return this.State;
            yield return this.Country;
            yield return this.ZipCode;
        }
    }
}
