using CMSGTechnical.Domain.Models;
using MediatR;
using MediatR;
using Microsoft.JSInterop;
using System.Text.Json;

namespace CMSGTechnical.Code
{

    public class BasketChangedEventArgs : EventArgs
    {
        public BasketDto Basket { get; set; }
    }


    public class BasketService : IAsyncDisposable
    {
        private const string StorageKey = "cmsg:basket";
        private const decimal DeliveryFee = 2m;
        private readonly IMediator _mediator;
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<BasketService>? _dotNetRef;
        private bool _initialized;

        public event EventHandler<BasketChangedEventArgs>? OnChange;

        public BasketDto Basket { get; private set; } = new();

        public decimal Subtotal => Basket.MenuItems.Sum(i => i.Price);
        public decimal Delivery => Basket.MenuItems.Any() ? DeliveryFee : 0m;
        public decimal Total => Subtotal + Delivery;

        public BasketService(IMediator mediator, IJSRuntime jsRuntime)
        {
            _mediator = mediator;
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            if (_initialized)
                return;

            _initialized = true;
            await LoadFromStorageAsync();

            if (!Basket.MenuItems.Any())
            {
                Basket = await _mediator.Send(new Mediator.Basket.GetBasket(1)) ?? new BasketDto();
                await PersistAsync();
            }

            _dotNetRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("cmsgBasketStorage.subscribe", _dotNetRef);

            NotifyChanged();
        }

        public async Task Add(MenuItemDto item)
        {
            Basket.MenuItems.Add(item);
            await PersistAndNotifyAsync();
        }

        public async Task Remove(MenuItemDto item)
        {
            var existing = Basket.MenuItems.LastOrDefault(i => i.Id == item.Id);
            if (existing != null)
            {
                Basket.MenuItems.Remove(existing);
                await PersistAndNotifyAsync();
            }
        }

        [JSInvokable]
        public async Task OnStorageChanged(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return;

            var deserialized = JsonSerializer.Deserialize<BasketDto>(json);
            if (deserialized is null)
                return;

            Basket = deserialized;
            NotifyChanged();
            await Task.CompletedTask;
        }

        private async Task PersistAndNotifyAsync()
        {
            await PersistAsync();
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            OnChange?.Invoke(this, new BasketChangedEventArgs { Basket = Basket });
        }

        private async Task PersistAsync()
        {
            await _jsRuntime.InvokeVoidAsync("cmsgBasketStorage.write", Basket);
        }

        private async Task LoadFromStorageAsync()
        {
            var stored = await _jsRuntime.InvokeAsync<BasketDto?>("cmsgBasketStorage.read");
            if (stored != null)
            {
                Basket = stored;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_dotNetRef != null)
            {
                await _jsRuntime.InvokeVoidAsync("cmsgBasketStorage.unsubscribe");
                _dotNetRef.Dispose();
            }
        }
    }
}
