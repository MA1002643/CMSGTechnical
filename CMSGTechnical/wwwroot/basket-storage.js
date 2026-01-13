// Basket persistence with localStorage and cross-tab sync
window.cmsgBasketStorage = (() => {
    const STORAGE_KEY = 'cmsg:basket';
    let subscribers = [];

    // Handle storage events from other tabs
    window.addEventListener('storage', (e) => {
        if (e.key === STORAGE_KEY && e.newValue) {
            subscribers.forEach(ref => {
                ref.invokeMethodAsync('OnStorageChanged', e.newValue);
            });
        }
    });

    return {
        read() {
            const json = localStorage.getItem(STORAGE_KEY);
            if (!json) return null;
            try {
                return JSON.parse(json);
            } catch {
                return null;
            }
        },

        write(basket) {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(basket));
            // Notify other subscribers in this tab
            subscribers.forEach(ref => {
                if (ref) {
                    ref.invokeMethodAsync('OnStorageChanged', JSON.stringify(basket));
                }
            });
        },

        subscribe(dotnetRef) {
            if (dotnetRef && !subscribers.includes(dotnetRef)) {
                subscribers.push(dotnetRef);
            }
        },

        unsubscribe() {
            subscribers = [];
        }
    };
})();
