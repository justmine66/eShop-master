using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Extensions
{
    /// <summary>
    /// Ling查询扩展类
    /// </summary>
    public static class LinqSelectExtensions
    {
        /// <summary>
        /// 根据选择器，从源数据集中筛选结果
        /// </summary>
        /// <typeparam name="TSource">源数据集项类型</typeparam>
        /// <typeparam name="TResult">结果数据集项类型</typeparam>
        /// <param name="enumerable">源数据集</param>
        /// <param name="selector">选择器</param>
        /// <returns>结果数据集</returns>
        public static IEnumerable<SelectTryResult<TSource, TResult>> SelectTry<TSource, TResult>(this IEnumerable<TSource> enumerable,
            Func<TSource, TResult> selector)
        {
            foreach (TSource ele in enumerable)
            {
                SelectTryResult<TSource, TResult> returnedValue;
                try
                {
                    returnedValue = new SelectTryResult<TSource, TResult>(ele, selector(ele), null);
                }
                catch (Exception ex)
                {
                    returnedValue = new SelectTryResult<TSource, TResult>(ele, default(TResult), ex);
                }
                yield return returnedValue;
            }
        }

        /// <summary>
        /// 根据异常处理函数，将源数据集中每项的异常转为为正常的结果项
        /// </summary>
        /// <typeparam name="TSource">源数据集项类型</typeparam>
        /// <typeparam name="TResult">结果数据集项类型</typeparam>
        /// <param name="enumerable">源数据集</param>
        /// <param name="exceptionHandler">异常处理函数</param>
        /// <returns></returns>
        public static IEnumerable<TResult> OnCaughtException<TSource, TResult>(this IEnumerable<SelectTryResult<TSource, TResult>> enumerable,
            Func<Exception, TResult> exceptionHandler)
        {
            return enumerable.Select(x => x.CaughtException == null ? x.Result : exceptionHandler(x.CaughtException));
        }

        /// <summary>
        /// 根据异常处理函数，将源数据集中每项的异常转为为正常的结果项
        /// </summary>
        /// <typeparam name="TSource">源数据集项类型</typeparam>
        /// <typeparam name="TResult">结果数据集项类型</typeparam>
        /// <param name="enumerable">源数据集</param>
        /// <param name="exceptionHandler">异常处理函数</param>
        /// <returns></returns>
        public static IEnumerable<TResult> OnCaughtException<TSource, TResult>(this IEnumerable<SelectTryResult<TSource, TResult>> enumerable, Func<TSource, Exception, TResult> exceptionHandler)
        {
            return enumerable.Select(x => x.CaughtException == null ? x.Result : exceptionHandler(x.Source, x.CaughtException));
        }

        /// <summary>
        /// 查询尝试结果聚合
        /// </summary>
        /// <typeparam name="TSource">源数据集项类型</typeparam>
        /// <typeparam name="TResult">结果数据集项类型</typeparam>
        public class SelectTryResult<TSource, TResult>
        {
            internal SelectTryResult(TSource source, TResult result, Exception exception)
            {
                this.Source = source;
                this.Result = result;
                this.CaughtException = exception;
            }

            public TSource Source { get; private set; }
            public TResult Result { get; private set; }
            public Exception CaughtException { get; private set; }
        }
    }
}
