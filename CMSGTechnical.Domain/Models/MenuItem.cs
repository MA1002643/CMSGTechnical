using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMSGTechnical.Domain.Interfaces;

namespace CMSGTechnical.Domain.Models
{
    public class MenuItem : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public int Order { get; set; } = 0;

        public MenuCategory Category { get; set; } // added to allow menu items to be categorised (Starter / Main / Dessert)

        public ICollection<MenuItem> ChildItems { get; set; } = new List<MenuItem>();
    }
}
