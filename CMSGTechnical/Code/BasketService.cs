using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Basket;
using CMSGTechnical.Mediator.Dtos;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace CMSGTechnical.Code
{
    public class BasketChangedEventArgs : EventArgs { public BasketDto Basket { get; set; } }
    public class BasketService
    {
        public event EventHandler<BasketChangedEventArgs> OnChange;
        public BasketDto Basket { get; }
        public BasketService(BasketDto basket) { Basket = basket; }
        public void Add(MenuItemDto item) // changed to void as async is not needed and awaits nothing.
        {
            Basket.MenuItems.Add(item);
            OnChange(this, new BasketChangedEventArgs() { Basket = Basket });
        }
        public void Remove(MenuItemDto item)
        {
            Basket.MenuItems.Remove(item);
            OnChange(this, new BasketChangedEventArgs() { Basket = Basket });
        }
    }
}