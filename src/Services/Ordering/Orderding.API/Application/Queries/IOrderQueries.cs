using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orderding.API.Application.Queries
{
    /// <summary>
    /// 订单查询
    /// </summary>
    public interface IOrderQueries
    {
        Task<dynamic> GetOrderAsync(int id);

        Task<IEnumerable<dynamic>> GetOrdersAsync();

        Task<IEnumerable<dynamic>> GetCardTypesAsync();
    }
}
