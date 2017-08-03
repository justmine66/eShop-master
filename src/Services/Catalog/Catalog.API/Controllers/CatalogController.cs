using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Catalog.API.Infrastructure;
using Microsoft.Extensions.Options;
using EventBus.Abstractions;
using IntegrationEventLogEF.Services;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Catalog.API.Model;
using Catalog.API.ViewModel;
using EventBus.Events;
using Catalog.API.IntegrationEvents.Events;
using Microsoft.EntityFrameworkCore.Storage;
using Catalog.API.IntegrationEvents;

namespace Catalog.API.Controllers
{
    /// <summary>
    /// 目录控制器
    /// </summary>
    [Route("api/v1/[controller]")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _catalogContext;
        private readonly CatalogSettings _settings;
        private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

        public CatalogController(
            CatalogContext Context,
            IOptionsSnapshot<CatalogSettings> settings,
            ICatalogIntegrationEventService catalogIntegrationEventService)
        {
            this._catalogContext = Context ?? throw new ArgumentNullException(nameof(Content));
            this._catalogIntegrationEventService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogIntegrationEventService));
            this._settings = settings.Value;
            ((DbContext)Context).ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        /// <summary>
        /// 分页获取目录数据集
        /// </summary>
        /// <param name="pageSize">页尺寸，即每页有多少项</param>
        /// <param name="pageIndex">页标，即第几页</param>
        /// <returns></returns>
        // GET api/v1/[controller]/items[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var totalItems = await _catalogContext.CatalogItems
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
                .OrderBy(c => c.Name)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = this.ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        /// <summary>
        /// 根据标识获取目录项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/v1/[controller]/items/3
        [HttpGet]
        [Route("items/id:int")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id < 0) return BadRequest();
            var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(c => c.Id == id);
            if (null != item) return Ok(item);
            return NotFound();
        }

        /// <summary>
        /// 分页获取目录数据集
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="pageSize">页尺寸，即每页有多少项</param>
        /// <param name="pageIndex">页标，即第几页</param>
        /// <returns></returns>
        // GET api/v1/[controller]/items/withname/samplename[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(
            string name,
            [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
                root = root.Where(c => c.Name.StartsWith(name));

            var totalItems = await root
                .LongCountAsync();

            var itemsOnPage = await root
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = this.ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        /// <summary>
        /// 分页获取目录数据集
        /// </summary>
        /// <param name="catalogTypeId">目录类型标识</param>
        /// <param name="catalogBrandId">目录品牌标识</param>
        /// <param name="pageSize">页尺寸，即每页有多少项</param>
        /// <param name="pageIndex">页标，即第几页</param>
        /// <returns></returns>
        // GET api/v1/[controller]/items/type/1/brand/null[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/type/{catalogTypeId}/brand/{catalogBrandId}")]
        public async Task<IActionResult> Items(
            int? catalogTypeId,
            int? catalogBrandId,
            [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

            if (catalogTypeId.HasValue)
                root.Where(c => c.CatalogTypeId == catalogTypeId);
            if (catalogBrandId.HasValue)
                root.Where(c => c.CatalogBrandId == catalogBrandId);

            var totalItems = await root
               .LongCountAsync();

            var itemsOnPage = await root
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            itemsOnPage = this.ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(
                pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        // POST api/v1/[controller]/create
        /// <summary>
        /// 创建目录项
        /// </summary>
        /// <param name="product">目录项</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateProduct([FromBody]CatalogItem product)
        {
            var item = new CatalogItem
            {
                CatalogBrandId = product.CatalogBrandId,
                CatalogTypeId = product.CatalogTypeId,
                Description = product.Description,
                Name = product.Name,
                PictureUri = product.PictureUri,
                Price = product.Price
            };
            _catalogContext.CatalogItems.Add(item);

            await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, null);
        }

        /// <summary>
        /// 获取目录类型数据集
        /// </summary>
        /// <returns></returns>
        // GET api/v1/[controller]/CatalogTypes
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var items = await _catalogContext.CatalogTypes
                .ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// 获取目录品牌数据集
        /// </summary>
        /// <returns></returns>
        // GET api/v1/[controller]/CatalogBrands
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var items = await _catalogContext.CatalogBrands
                .ToListAsync();

            return Ok(items);
        }

        //POST api/v1/[controller]/update
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody]CatalogItem productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogItems
                .SingleOrDefaultAsync(c => c.Id == productToUpdate.Id);
            if (null == catalogItem)
                return NotFound(new { Message = $"未找到ID为{productToUpdate.Id}的目录项" });

            var oldPrice = catalogItem.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            //更新当前产品
            catalogItem = productToUpdate;
            _catalogContext.CatalogItems.Update(catalogItem);
            //价格改变，保存和发布事件。
            if (raiseProductPriceChangedEvent)
            {
                //创建价格改变事件
                var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice);
                //保存目录数据和事件发布日志--此步骤需要通过本地事务保证原子性。
                await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);
                //发布事件
                await _catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            }
            else
            {
                await _catalogContext.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetItemById), new { id = productToUpdate.Id }, null);
        }

        //DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _catalogContext.CatalogItems.SingleOrDefaultAsync(c => c.Id == id);
            if (null == product) return NotFound();

            _catalogContext.CatalogItems.Remove(product);
            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// 构造图片路径
        /// </summary>
        /// <param name="items">From目录数据集</param>
        /// <returns>To目录数据集</returns>
        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> items)
        {
            var baseUri = _settings.PicBaseUrl;
            items.ForEach(x =>
            {
                x.PictureUri = x.PictureUri.Replace("http://externalcatalogbaseurltobereplaced", baseUri);
            });

            return items;
        }
    }
}
