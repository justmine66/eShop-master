using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basket.API.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Basket.API.Infrastructure.Respositorys
{
    public class RedisBasketRespository : IBasketRespository
    {
        private readonly ILogger<RedisBasketRespository> _logger;

        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisBasketRespository(ILogger<RedisBasketRespository> logger, ConnectionMultiplexer redis)
        {
            this._logger = logger;
            this._redis = redis ?? throw new ArgumentNullException(nameof(redis));
            this._database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await this._database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            RedisValue data = await this._database.StringGetAsync(customerId);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public IEnumerable<string> GetUsers()
        {
            IServer server = this.GetServer();
            IEnumerable<RedisKey> data = server.Keys();
            return data?.Select(k => k.ToString());
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            bool created = await this._database.StringSetAsync(basket.BuyId, JsonConvert.SerializeObject(basket));

            if (!created)
            {
                this._logger.LogError("更新购物车出现异常");
            }

            this._logger.LogInformation("更新购物车出现异常");

            return await this.GetBasketAsync(basket.BuyId);
        }

        private IServer GetServer()
        {
            var endpoints = this._redis.GetEndPoints();
            return this._redis.GetServer(endpoints[0]);
        }
    }
}
