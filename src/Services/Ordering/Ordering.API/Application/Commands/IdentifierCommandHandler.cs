using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Ordering.Infrastructure.Idempotency;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// 处理重复的请求，确保幂等性，一个基本的实现
    /// </summary>
    /// <typeparam name="T">未重复请求的命令处理类型</typeparam>
    /// <typeparam name="R">内部命令处理的返回值</typeparam>
    public class IdentifierCommandHandler<T, R> : IAsyncRequestHandler<IdentifiedCommand<T, R>, R>
        where T : IRequest<R>
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;

        public IdentifierCommandHandler(IMediator mediator, IRequestManager requestManager)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
        }

        /// <summary>
        /// 为重复的请求创建结果
        /// </summary>
        /// <returns></returns>
        protected virtual R CreateResultForDuplicateRequest()
        {
            return default(R);
        }

        public async Task<R> Handle(IdentifiedCommand<T, R> message)
        {
            var alreadyExists = await _requestManager.ExistAsync(message.Id);
            if (alreadyExists)
            {
                return this.CreateResultForDuplicateRequest();
            }
            else
            {
                await _requestManager.CreateRequestForCommandAsync<T>(message.Id);

                var result =await this._mediator.Send(message.Command);

                return result;
            }
        }
    }
}
