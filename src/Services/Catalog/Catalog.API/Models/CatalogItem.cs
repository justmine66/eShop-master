using Catalog.API.Infrastructure.Exceptions;
using System;

namespace Catalog.API.Model
{
    public class CatalogItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureFileName { get; set; }

        public string PictureUri { get; set; }

        public int CatalogTypeId { get; set; }

        public CatalogType CatalogType { get; set; }

        public int CatalogBrandId { get; set; }

        public CatalogBrand CatalogBrand { get; set; }

        /// <summary>
        /// 有效库存
        /// </summary>
        public int AvailableStock { get; set; }

        public int RestockThreshold { get; set; }

        /// <summary>
        /// 仓库实际能存储的最大单位库存阈值
        /// </summary>
        public int MaxStockThreshold { get; set; }

        public bool OnReorder { get; set; }

        public CatalogItem() { }

        /// <summary>
        /// 移除库存
        /// </summary>
        /// <param name="quantityDesired">期望移除数量</param>
        /// <returns>实际移除的数量</returns>
        public int RemoveStock(int quantityDesired)
        {
            if (this.AvailableStock == 0)
            {
                throw new CatalogDomainException($"Empty stock, product item {Name} is sold out");
            }
            if (quantityDesired <= 0)
            {
                throw new CatalogDomainException($"Item units desired should be greater than zero");
            }

            int removed = Math.Min(quantityDesired, this.AvailableStock);
            this.AvailableStock -= removed;
            return removed;
        }

        /// <summary>
        /// 添加库存
        /// </summary>
        /// <param name="quantity">期望新增的数量</param>
        /// <returns>实际新增的数量</returns>
        public int AddStock(int quantityDesired)
        {
            int original = this.AvailableStock;
            if ((this.AvailableStock + quantityDesired > this.MaxStockThreshold))//
            {
                this.AvailableStock += (this.MaxStockThreshold - this.AvailableStock);
            }
            else
            {
                this.AvailableStock += quantityDesired;
            }

            this.OnReorder = false;

            return this.AvailableStock - original;
        }
    }
}
