using CMSGTechnical.Domain.Interfaces;
using CMSGTechnical.Domain.Models;
using CMSGTechnical.Mediator.Dtos;
using MediatR;

namespace CMSGTechnical.Mediator.Menu
{
    public record GetMenuItem(int Id) : IRequest<MenuItemDto?>;

    public class GetMenuItemHandler : IRequestHandler<GetMenuItem, MenuItemDto?>
    {

        private IRepo<MenuItem> MenuItems { get; }

        public GetMenuItemHandler(IRepo<MenuItem> menuItems)
        {
            MenuItems = menuItems;
        }

        public async Task<MenuItemDto?> Handle(GetMenuItem request, CancellationToken cancellationToken)
        {
            var r  = await MenuItems.Get(request.Id, cancellationToken);
            return r.ToDto();
        }
    }



}
