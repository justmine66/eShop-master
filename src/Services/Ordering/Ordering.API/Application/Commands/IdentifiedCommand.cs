using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 可识别的命令
    /// </summary>
    /// <typeparam name="T">命令</typeparam>
    /// <typeparam name="R"></typeparam>
    public class IdentifiedCommand<T, R> : IRequest<R>
        where T : IRequest<R>
    {
        /// <summary>
        /// 命令
        /// </summary>
        public T Command { get; }

        /// <summary>
        /// 请求ID，用于确保幂等性
        /// </summary>
        public Guid Id { get; }

        public IdentifiedCommand(T command, Guid id)
        {
            this.Command = command;
            this.Id = id;
        }
    }
}
