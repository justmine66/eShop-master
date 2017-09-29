using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Ordering.Infrastructure.Idempotency;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 幂等命令处理器
    /// 说明：对于重复的请求，确保命令消费的幂等性。
    /// </summary>
    /// <typeparam name="TCommand">幂等命令</typeparam>
    /// <typeparam name="TResult">响应结果</typeparam>
    public class IdentifierCommandHandler<TCommand, TResult> : IAsyncRequestHandler<IdentifiedCommand<TCommand, TResult>, TResult>
        where TCommand : IRequest<TResult>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public IdentifierCommandHandler(IMediator mediator, IRequestManager requestManager)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
        }

        /// <summary>
        /// 为重复的请求创建响应结果
        /// </summary>
        /// <returns>响应结果</returns>
        protected virtual TResult CreateResultForDuplicateRequest()
        {
            return default(TResult);
        }

        /// <summary>
        /// 处理幂等命令
        /// </summary>
        /// <param name="message">幂等命令</param>
        /// <returns></returns>
        public async Task<TResult> Handle(IdentifiedCommand<TCommand, TResult> identifiedCommand)
        {
            var alreadyExists = await _requestManager.ExistAsync(identifiedCommand.Id);
            if (alreadyExists)
            {
                return this.CreateResultForDuplicateRequest();
            }
            else
            {
                await _requestManager.CreateRequestForCommandAsync<TCommand>(identifiedCommand.Id);

                var result = await this._mediator.Send(identifiedCommand.Command);

                return result;
            }
        }
    }
}
