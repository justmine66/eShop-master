using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 幂等命令
    /// </summary>
    /// <typeparam name="TCommand">命令</typeparam>
    /// <typeparam name="TResult">结果</typeparam>
    public class IdentifiedCommand<TCommand, TResult> : IRequest<TResult>
        where TCommand : IRequest<TResult>
    {
        /// <summary>
        /// 命令
        /// </summary>
        public TCommand Command { get; }

        /// <summary>
        /// 请求ID，用于确保幂等性
        /// </summary>
        public Guid Id { get; }

        public IdentifiedCommand(TCommand command, Guid id)
        {
            this.Command = command;
            this.Id = id;
        }
    }
}
