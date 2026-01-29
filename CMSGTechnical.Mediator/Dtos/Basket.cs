using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;

namespace CMSGTechnical.Mediator.Dtos
{
    public class BasketDto
    {
        public int Id { get; set; }
        public ICollection<MenuItemDto> MenuItems { get; set; } = new List<MenuItemDto>();

        public IEnumerable<(MenuItemDto Item, int Quantity)> GroupedItems => // group basket items by menu item and get quantity for display.
            MenuItems
                .GroupBy(m => m.Id)
                .Select(g => (Item: g.First(), Quantity: g.Count()));

        public int UserId { get; set; }

        public decimal Subtotal => MenuItems.Sum(i => i.Price); // get the subtotal before delivery fee.
        public decimal DeliveryFee => MenuItems.Count != 0 ? 2m : 0m; // check if the basket is empty before applying the fee.
        public decimal Total => Subtotal + DeliveryFee; // total cost.

    }


    public static class BasketExtensions
    {


        public static IEnumerable<BasketDto> ToDto(this IEnumerable<Domain.Models.Basket> models) =>
            models.Select(i => i.ToDto()).ToArray();

        public static BasketDto ToDto(this Domain.Models.Basket model)
        {
            return new BasketDto()
            {
                Id = model.Id,
                MenuItems = model.MenuItems.ToDto().ToList(),
                UserId = model.UserId
            };
        }

    }

}
