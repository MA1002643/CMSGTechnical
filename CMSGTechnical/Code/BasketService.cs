using CMSGTechnical.Mediator.Dtos;
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
        private bool _jsInitialized;

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

            if (!Basket.MenuItems.Any())
            {
                Basket = await _mediator.Send(new Mediator.Basket.GetBasket(1)) ?? new BasketDto();
            }

            NotifyChanged();
        }

        public async Task InitializeJsAsync()
        {
            if (_jsInitialized)
                return;

            _jsInitialized = true;

            try
            {
                await LoadFromStorageAsync();
                NotifyChanged();

                _dotNetRef = DotNetObjectReference.Create(this);
                await _jsRuntime.InvokeVoidAsync("cmsgBasketStorage.subscribe", _dotNetRef);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"JS Interop error: {ex.Message}");
            }
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

            try
            {
                var deserialized = JsonSerializer.Deserialize<BasketDto>(json);
                if (deserialized is null)
                    return;

                // Only called from cross-tab storage events, safe to replace basket
                Basket = deserialized;
                NotifyChanged();
            }
            catch
            {
                // Silently fail if deserialization fails
            }
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
            try
            {
                await _jsRuntime.InvokeVoidAsync("cmsgBasketStorage.write", Basket);
            }
            catch
            {
                // Silently fail if JS interop not ready
            }
        }

        private async Task LoadFromStorageAsync()
        {
            try
            {
                var stored = await _jsRuntime.InvokeAsync<BasketDto?>("cmsgBasketStorage.read");
                if (stored != null)
                {
                    Basket = stored;
                }
            }
            catch
            {
                // Silently fail if JS interop not ready
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_dotNetRef != null)
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("cmsgBasketStorage.unsubscribe");
                }
                catch { }
                _dotNetRef.Dispose();
            }
        }
    }
}
