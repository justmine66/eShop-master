using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// 实体
    /// </summary>
    public abstract class Entity
    {
        #region [ 私有字段和构造函数重载 ]

        int? _requestHashCode;//请求哈希码，缓存避免每一次都重复计算哈希码，提高性能。
        int _Id;//标识

        private List<INotification> _domainEvents;//领域事件列表

        #endregion

        #region [ 公共属性 ]

        /// <summary>
        /// 身份标识
        /// </summary>
        public virtual int Id
        {
            get { return this._Id; }
            protected set { this._Id = value; }
        }

        /// <summary>
        /// 领域事件列表
        /// </summary>
        public List<INotification> DomainEvents => this._domainEvents;

        #endregion

        #region [ 命令方法 ]

        /// <summary>
        /// 添加领域事件
        /// </summary>
        /// <param name="eventItem"></param>
        public void AddDomainEvent(INotification eventItem)
        {
            this._domainEvents = this._domainEvents ?? new List<INotification>();
            this._domainEvents.Add(eventItem);
        }

        /// <summary>
        /// 移除领域事件
        /// </summary>
        /// <param name="eventItem"></param>
        public void RemoveDomainEvent(INotification eventItem)
        {
            if (null == _domainEvents) return;
            _domainEvents.Remove(eventItem);
        }

        #endregion

        #region [ 内部方法 ]

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsTransient()
        {
            return this._Id == default(int);
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj">实体,派生自Entity</param>
        /// <returns>是否相等</returns>
        public override bool Equals(object obj)
        {
            if (null == obj || !(obj is Entity))
                return false;

            if (object.ReferenceEquals(this, obj))//引用相等
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            var item = obj as Entity;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;//标识相等即代表两个实体相等.
        }

        /// <summary>
        /// 获取哈希码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.IsTransient())//实体为空
                return base.GetHashCode();
            else
            {
                if (!this._requestHashCode.HasValue)
                    this._requestHashCode = this.Id.GetHashCode() ^ 31;
                return this._requestHashCode.Value;
            }
        }

        #endregion

        #region [ 操作符 ]

        /// <summary>
        /// 等于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Entity left, Entity right)
        {
            if (object.Equals(left, null))
                return object.Equals(right, null);
            else
                return object.Equals(left, right);
        }

        /// <summary>
        /// 不等于
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        #endregion
    }
}
