using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;

namespace CMSGTechnical.Mediator.Dtos
{
    public class BasketDto
    {
        public int Id { get; set; }
        public ICollection<MenuItemDto> MenuItems { get; set; } = new List<MenuItemDto>();

        public int UserId { get; set; }

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
