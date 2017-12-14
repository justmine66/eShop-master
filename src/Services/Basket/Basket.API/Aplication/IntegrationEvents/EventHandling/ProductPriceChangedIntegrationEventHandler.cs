using Basket.API.Aplication.IntegrationEvents.Events;
using Basket.API.Infrastructure.Respositorys;
using Basket.API.Models;
using EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Aplication.IntegrationEvents.EventHandling
{
    public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
    {
        private readonly IBasketRespository _repository;

        public ProductPriceChangedIntegrationEventHandler(IBasketRespository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task HandleAsync(ProductPriceChangedIntegrationEvent @event)
        {
            IEnumerable<string> users = this._repository.GetUsers();
            foreach (var user in users)
            {
                CustomerBasket basket = await this._repository.GetBasketAsync(user);
                await this.UpdatePriceInBasketItems(@event.ProductId, @event.NewPrice, @event.OldPrice, basket);
            }
        }

        private async Task UpdatePriceInBasketItems(int productId, decimal newPrice, decimal oldPrice, CustomerBasket basket)
        {
            List<BasketItem> itemsToUpdate = basket.Items?
                .Where(item => int.TryParse(item.ProductId, out int iProductId) && iProductId == productId)
                .ToList();

            if (itemsToUpdate != null)
            {
                foreach (var item in itemsToUpdate)
                {
                    if (item.UnitPrice == oldPrice)
                    {
                        decimal originalPrice = item.UnitPrice;
                        item.UnitPrice = newPrice;
                        item.OldUnitPrice = originalPrice;
                    }
                }

                await this._repository.UpdateBasketAsync(basket);
            }
        }
    }
}
