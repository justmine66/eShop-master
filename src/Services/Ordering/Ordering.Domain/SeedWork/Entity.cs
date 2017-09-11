using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// 实体抽象
    /// </summary>
    public abstract class Entity
    {
        int? _requestHashCode;
        int _Id;

        private List<INotification> _domainEvents;

        public virtual int Id
        {
            get { return this._Id; }
            protected set { this._Id = value; }
        }

        public List<INotification> DomainEvents => this._domainEvents;
        public void AddDomainEvent(INotification eventItem)
        {
            this._domainEvents = this._domainEvents ?? new List<INotification>();
            this._domainEvents.Add(eventItem);
        }
        public void RemoveDomainEvent(INotification eventItem)
        {
            if (null == _domainEvents) return;
            _domainEvents.Remove(eventItem);
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsTransient()
        {
            return this._Id == default(int);
        }

        public override bool Equals(object obj)
        {
            if (null == obj || !(obj is Entity))
                return false;

            if (object.ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            Entity item = obj as Entity;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (this.IsTransient())//Id为空
                return base.GetHashCode();
            else
            {
                if (!this._requestHashCode.HasValue)
                    this._requestHashCode = this.Id.GetHashCode() ^ 31;
                return this._requestHashCode.Value;
            }
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (object.Equals(left, null))
                return object.Equals(right, null);
            else
                return object.Equals(left, right);
        }
        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
