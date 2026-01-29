using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using CMSGTechnical.Mediator.Dtos;
using Xunit;

namespace CMSGTechnical.Mediator.Tests
{
    public class BasketDtoTests
    {
        [Fact]
        public void Empty_basket_has_zero_subtotal_and_no_delivery_fee()
        {
            var basket = new BasketDto();

            Assert.Equal(0m, basket.Subtotal);
            Assert.Equal(0m, basket.DeliveryFee);
            Assert.Equal(0m, basket.Total);
        }

        [Fact]
        public void Subtotal_is_sum_of_item_prices()
        {
            var basket = new BasketDto();
            basket.MenuItems.Add(new MenuItemDto { Id = 1, Name = "A", Price = 10m });
            basket.MenuItems.Add(new MenuItemDto { Id = 2, Name = "B", Price = 5.5m });

            Assert.Equal(15.50m, basket.Subtotal);
            Assert.Equal(2m, basket.DeliveryFee);
            Assert.Equal(17.50m, basket.Total);
        }

        [Fact]
        public void GroupedItems_groups_same_id_and_counts_quantity()
        {
            var basket = new BasketDto();

            basket.MenuItems.Add(new MenuItemDto { Id = 9, Name = "Cake", Price = 5 });
            basket.MenuItems.Add(new MenuItemDto { Id = 9, Name = "Cake", Price = 5 });
            basket.MenuItems.Add(new MenuItemDto { Id = 9, Name = "Cake", Price = 5 });


            var grouped = basket.GroupedItems.ToList();

            Assert.Single(grouped);
            Assert.Equal(9, grouped[0].Item.Id);
            Assert.Equal(3, grouped[0].Quantity);
        }

        [Fact]
        public void GroupedItems_creates_serperate_groups_for_different_ids()
        {
            var basket = new BasketDto();

            basket.MenuItems.Add(new MenuItemDto { Id = 1, Name = "A", Price = 1 });
            basket.MenuItems.Add(new MenuItemDto { Id = 2, Name = "B", Price = 5 });
            basket.MenuItems.Add(new MenuItemDto { Id = 2, Name = "B", Price = 5 });
            basket.MenuItems.Add(new MenuItemDto { Id = 3, Name = "C", Price = 7 });
            basket.MenuItems.Add(new MenuItemDto { Id = 3, Name = "C", Price = 7 });
            basket.MenuItems.Add(new MenuItemDto { Id = 3, Name = "C", Price = 7 });

            var grouped = basket.GroupedItems.OrderBy(x => x.Item.Id).ToList();

            Assert.Equal(3, grouped.Count);
            Assert.Equal(1, grouped[0].Item.Id);
            Assert.Equal(1, grouped[0].Quantity);
            Assert.Equal(2, grouped[1].Item.Id);
            Assert.Equal(2, grouped[1].Quantity);
            Assert.Equal(3, grouped[2].Item.Id);
            Assert.Equal(3, grouped[2].Quantity);


        }
    }
}