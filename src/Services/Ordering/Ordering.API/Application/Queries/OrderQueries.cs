using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private string _connectionString = string.Empty;

        public OrderQueries(string constr)
        {
            _connectionString =
                !string.IsNullOrWhiteSpace(constr)
                ? constr
                : throw new ArgumentNullException(nameof(constr));
        }

        public async Task<IEnumerable<dynamic>> GetCardTypesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<dynamic>("SELECT * FROM ordering.cardtypes");
            }
        }

        public async Task<dynamic> GetOrderAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var result = await connection.QueryAsync(@"", new { id });

                if (result.AsList().Count == 0)
                    throw new KeyNotFoundException();

                return this.MapOrderItems(result);
            }
        }

        public async Task<IEnumerable<dynamic>> GetOrdersAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<dynamic>("");
            }
        }

        private dynamic MapOrderItems(dynamic result)
        {
            dynamic order = new ExpandoObject();
            return order;
        }
    }
}
