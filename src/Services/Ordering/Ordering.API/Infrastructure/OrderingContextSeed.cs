﻿using IntegrationEventLogEF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.API.Extensions;
using Ordering.Domain.AggregatesModel.BuyerAggregate;
using Ordering.Domain.AggregatesModel.OrderAggregate;
using Ordering.Infrastructure;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Infrastructure
{
    /// <summary>
    /// 订单子域数据播种
    /// </summary>
    public class OrderingContextSeed
    {
        /// <summary>
        /// 订单子域数据播种
        /// </summary>
        /// <param name="applicationBuilder">应用程序</param>
        /// <param name="env">环境参数</param>
        /// <param name="loggerFactory">日志工厂</param>
        /// <returns></returns>
        public async Task SeedAsync(
            OrderingContext context,
            IHostingEnvironment env,
            IOptions<OrderingSettings> settings,
            ILogger<OrderingContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderingContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = settings.Value.UseCustomizationData;
                var contentRootPath = env.ContentRootPath;

                using (context)
                {
                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData
                                                ? GetCardTypesFromFile(contentRootPath, logger)
                                                : GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData
                                                ? GetOrderStatusFromFile(contentRootPath, logger)
                                                : GetPredefinedOrderStatuses());

                        await context.SaveChangesAsync();
                    }
                }
            });
        }

        //从文件获取银行卡类型
        IEnumerable<CardType> GetCardTypesFromFile(string contentRootPath, ILogger log)
        {
            string csvFileCardTypes = Path.Combine(contentRootPath, "Setup", "CardTypes.csv");

            if (!File.Exists(csvFileCardTypes))
            {
                return GetPredefinedCardTypes();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "CardType" };
                csvheaders = GetHeaders(requiredHeaders, csvFileCardTypes);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                return GetPredefinedCardTypes();
            }

            int id = 1;
            return File.ReadAllLines(csvFileCardTypes)
                                        .Skip(1) // skip header column
                                        .SelectTry(x => CreateCardType(x, ref id))
                                        .OnCaughtException(ex => { log.LogError(ex.Message); return null; })
                                        .Where(x => x != null);
        }
        //创建银行卡类型
        CardType CreateCardType(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Orderstatus is null or empty");
            }

            return new CardType(id++, value.Trim('"').Trim());
        }
        //获取预定义的银行卡类型
        IEnumerable<CardType> GetPredefinedCardTypes()
        {
            yield return CardType.aPlay;
            yield return CardType.weChat;
            yield return CardType.unionPlay;
        }
        //从文件获取订单状态
        IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentRootDir, ILogger logger)
        {
            string csvFile = Path.Combine(contentRootDir, "SetUp", "OrderStatus.csv");

            if (!File.Exists(csvFile))
            {
                return GetPredefinedOrderStatuses();
            }

            string[] csvHeaders;
            try
            {
                var requiredHeaders = new string[] { "OrderStatus" };
                csvHeaders = GetHeaders(requiredHeaders, csvFile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return GetPredefinedOrderStatuses();
            }

            int id = 1;
            return csvHeaders
                .Skip(1)
                .SelectTry(value => CreateOrderStatus(value, ref id))
                .OnCaughtException(ex => { logger.LogError(ex.Message); return null; })
                .Where(i => i != null);
        }
        //创建订单状态
        OrderStatus CreateOrderStatus(string value, ref int id)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new Exception("Orderstatus is null or empty");
            }

            return new OrderStatus(id++, value.Trim('"').Trim().ToLowerInvariant());
        }
        //获取预定义的订单状态集合
        IEnumerable<OrderStatus> GetPredefinedOrderStatuses()
        {
            yield return OrderStatus.Submitted;
            yield return OrderStatus.AwaitingValidation;
            yield return OrderStatus.StockConfirmed;
            yield return OrderStatus.Paid;
            yield return OrderStatus.Shipped;
            yield return OrderStatus.Cancelled;
        }
        //获取列头
        string[] GetHeaders(string[] requiredHeaders, string csvFile)
        {
            string[] csvHeaders = File.ReadLines(csvFile).FirstOrDefault().ToLowerInvariant().Split(",");

            if ((csvHeaders.Count() ^ requiredHeaders.Count()) != 0)
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvHeaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvHeaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvHeaders;
        }

        //创建重试策略
        Policy CreatePolicy(ILogger logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>()
                 .WaitAndRetryAsync(
                 retryCount: retries,
                 sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                 onRetry: (exception, timeSpan, retry, ctx) =>
                 {
                     logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                 });
        }
    }
}
