using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.SeedWork
{
    /// <summary>
    /// 仓库接口
    /// </summary>
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
