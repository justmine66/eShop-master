using System;
using System.Collections.Generic;
using System.Text;
using Ordering.Domain.SeedWork;
using System.Threading.Tasks;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Ordering.Infrastructure.Repositories
{
    /// <summary>
    /// 买家仓库
    /// </summary>
    public class BuyerRepository
        : IBuyerRepository
    {
        private readonly OrderingContext _context;
        public IUnitOfWork UnitOfWork => this._context;

        public BuyerRepository(OrderingContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Buyer Add(Buyer buyer)
        {
            if (buyer.IsTransient())
            {
                return this._context.Buyers
                    .Add(buyer)
                    .Entity;
            }
            else {
                return buyer;
            }
        }

        public async Task<Buyer> FindAsync(string buyerIdentityGuid)
        {
            var buyer = await this._context.Buyers
                .Include(b => b.PaymentMethods)
                .SingleOrDefaultAsync(b => b.IdentityGuid == buyerIdentityGuid);

            return buyer;
        }

        public Buyer Update(Buyer buyer)
        {
            return this._context
                .Buyers
                .Update(buyer)
                .Entity;
        }
    }
}
