using Ordering.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Idempotency
{
    /// <summary>
    /// 请求管理器，保证幂等性
    /// </summary>
    public class RequestManager : IRequestManager
    {
        private readonly OrderingContext _context;

        public RequestManager(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await this.ExistAsync(id);
            var request = exists ?
                throw new OrderingDomainException($"Reuest with {id} already exists") :
                new ClientRequest()
                {
                    Id = id,
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            _context.Add(request);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context
                .FindAsync<ClientRequest>(id);
            return request != null;
        }
    }
}
