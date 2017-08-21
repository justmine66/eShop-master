using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries
{
    /// <summary>
    /// 订单查询
    /// </summary>
    public class OrderQueries
        : IOrderQueries
    {
        private string _connectionString = string.Empty;

        public OrderQueries(string conStr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(conStr)
                ? conStr
                : throw new ArgumentNullException(nameof(conStr));
        }

        public Task<IEnumerable<dynamic>> GetCardTypesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetOrderAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var result = connection.QueryAsync<dynamic>(
                    @"",
                    new { id });

                if (result.AsList())
                {

                }
            }
        }

        public Task<IEnumerable<dynamic>> GetOrdersAsync()
        {
            throw new NotImplementedException();
        }

        //映射订单项
        private dynamic MapOrderItems(dynamic result)
        {
            dynamic order = new ExpandoObject();

            return order;
        }
    }
}
