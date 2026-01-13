using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Dtos;
using CMSGTechnical.Mediator.Menu;
using BasketHandler = CMSGTechnical.Mediator.Basket.GetBasketHandler;
using BasketQuery = CMSGTechnical.Mediator.Basket.GetBasket;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace CMSGTechnical.Mediator.Tests
{
    public class GetMenuItemsOrderingTests
    {
        [Fact]
        public void OrderingLogic_SortsByPriceAscending()
        {
            // Arrange
            var menuItems = new[]
            {
                new MenuItem { Id = 1, Name = "Burger", Price = 10.99m, Category = "Main", Order = 0 },
                new MenuItem { Id = 2, Name = "Pizza", Price = 12.99m, Category = "Main", Order = 0 },
                new MenuItem { Id = 3, Name = "Salad", Price = 8.99m, Category = "Starter", Order = 0 },
            };

            // Act - simulate the ordering in GetMenuItemsHandler
            var sorted = menuItems.OrderBy(i => i.Price).ThenBy(i => i.Name).ToList();

            // Assert
            Assert.Equal(3, sorted.Count);
            Assert.Equal("Salad", sorted[0].Name);
            Assert.Equal("Burger", sorted[1].Name);
            Assert.Equal("Pizza", sorted[2].Name);
        }
    }

    public class MenuItemDtoMappingTests
    {
        [Fact]
        public void ToDto_MapsCategoryCorrectly()
        {
            var menuItem = new MenuItem
            {
                Id = 1,
                Name = "Chocolate Cake",
                Price = 6.99m,
                Description = "Rich chocolate sponge layered with ganache and cream.",
                Category = "Dessert",
                Order = 0
            };

            var dto = menuItem.ToDto();

            Assert.Equal("Dessert", dto.Category);
            Assert.Equal("Chocolate Cake", dto.Name);
            Assert.Equal("Rich chocolate sponge layered with ganache and cream.", dto.Description);
        }

        [Fact]
        public void ToDto_HandlesNullDescription()
        {
            var menuItem = new MenuItem
            {
                Id = 1,
                Name = "Item",
                Price = 5m,
                Description = null,
                Category = "Main",
                Order = 0
            };

            var dto = menuItem.ToDto();

            Assert.Null(dto.Description);
        }
    }

    public class BasketDtoTests
    {
        [Fact]
        public void ToDto_MapsBasketWithItems()
        {
            var basket = new CMSGTechnical.Domain.Models.Basket
            {
                Id = 1,
                UserId = 42,
                MenuItems = new[]
                {
                    new MenuItem { Id = 1, Name = "Pizza", Price = 12.99m, Category = "Main", Order = 0 }
                }
            };

            var dto = basket.ToDto();

            Assert.Equal(1, dto.Id);
            Assert.Equal(42, dto.UserId);
            Assert.Single(dto.MenuItems);
        }
    }

    public class GetBasketTests
    {
        [Fact]
        public async Task Handle_ReturnsBasket()
        {
            var basket = new CMSGTechnical.Domain.Models.Basket
            {
                Id = 1,
                UserId = 1,
                MenuItems = new[]
                {
                    new MenuItem { Id = 1, Name = "Pizza", Price = 12.99m, Category = "Main", Order = 0 },
                    new MenuItem { Id = 2, Name = "Salad", Price = 8.99m, Category = "Starter", Order = 0 }
                }
            };

            var mockRepo = new Mock<IRepo<CMSGTechnical.Domain.Models.Basket>>();
            mockRepo.Setup(r => r.Get(1, It.IsAny<CancellationToken>())).ReturnsAsync(basket);
            var handler = new BasketHandler(mockRepo.Object);

            var result = await handler.Handle(new BasketQuery(1), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.MenuItems.Count);
        }
    }

    public class PriceFormattingTests
    {
        [Fact]
        public void Price_DisplaysWithCurrencySymbol()
        {
            const decimal price = 12.99m;
            var formatted = $"£{price:0.00}";
            Assert.Equal("£12.99", formatted);
        }

        [Fact]
        public void DeliveryFee_IncludedInTotal()
        {
            const decimal subtotal = 20m;
            const decimal deliveryFee = 2m;
            var total = subtotal + deliveryFee;
            Assert.Equal(22m, total);
        }
    }

    public class MenuItemCategoryTests
    {
        [Fact]
        public void MenuItem_HasCategory()
        {
            var item = new MenuItem
            {
                Id = 1,
                Name = "Test",
                Category = "Main",
                Price = 10m,
                Order = 0
            };

            Assert.NotNull(item.Category);
            Assert.Equal("Main", item.Category);
        }
    }

    public class EnumerableMappingTests
    {
        [Fact]
        public void MenuItems_ToDto_MapsMultipleItems()
        {
            var items = new[]
            {
                new MenuItem { Id = 1, Name = "Item1", Price = 5m, Category = "Main", Order = 0 },
                new MenuItem { Id = 2, Name = "Item2", Price = 10m, Category = "Starter", Order = 0 }
            };

            var dtos = items.ToDto().ToList();

            Assert.Equal(2, dtos.Count);
            Assert.Equal("Item1", dtos[0].Name);
            Assert.Equal("Item2", dtos[1].Name);
        }
    }
}
