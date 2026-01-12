using CMSGTechnical.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CMSGTechnical.Mediator.Dtos
{
    public class MenuItemDto
    {

        public int Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int Order { get; set; } = 0;

        public ICollection<MenuItemDto> ChildItems { get; set; } = new List<MenuItemDto>();

    }


    public static class MenuItemExtensions
    {

        public static IEnumerable<MenuItemDto> ToDto(this IEnumerable<MenuItem> items) =>
            items.Select(i => i.ToDto());

        public static MenuItemDto ToDto(this MenuItem menuItem)
        {

            return new MenuItemDto()
            {
                Price = menuItem.Price,
                ChildItems = menuItem.ChildItems.ToDto().ToList(),
                Description = menuItem.Description,
                Name = menuItem.Name,
                Id = menuItem.Id,
                Order = menuItem.Order,

            };
        }

    }

}

