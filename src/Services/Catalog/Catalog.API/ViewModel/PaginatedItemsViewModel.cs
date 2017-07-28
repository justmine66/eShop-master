using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.ViewModel
{
    /// <summary>
    /// 分页项集视图模型
    /// </summary>
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        /// <summary>
        /// 页标，即第几页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页尺寸，即每页有多少项
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总项数
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// 数据集
        /// </summary>
        public IEnumerable<TEntity> Data { get; set; }

        /// <summary>
        /// 初始化分页项集实例
        /// </summary>
        /// <param name="pageIndex">页标，即第几页</param>
        /// <param name="pageSize">页尺寸，即每页有多少项</param>
        /// <param name="count">总项数，即总共有多少条数据项</param>
        /// <param name="data">数据集</param>
        public PaginatedItemsViewModel(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
        }
    }
}
