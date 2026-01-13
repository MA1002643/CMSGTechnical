// Basket persistence with localStorage and cross-tab sync
window.cmsgBasketStorage = (() => {
  const STORAGE_KEY = "cmsg:basket";
  let subscribers = [];

  // Handle storage events from other tabs/windows
  window.addEventListener("storage", (e) => {
    if (e.key === STORAGE_KEY && e.newValue) {
      try {
        subscribers.forEach((ref) => {
          if (ref) {
            ref
              .invokeMethodAsync("OnStorageChanged", e.newValue)
              .catch((err) => {
                console.warn("Failed to invoke OnStorageChanged:", err);
              });
          }
        });
      } catch (err) {
        console.error("Storage event handler error:", err);
      }
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
      try {
        const json = JSON.stringify(basket);
        localStorage.setItem(STORAGE_KEY, json);
        // Do NOT notify subscribers here - let PersistAndNotifyAsync handle it
        // Only storage events from other tabs should trigger OnStorageChanged
      } catch (err) {
        console.error("Write to storage failed:", err);
      }
    },

    subscribe(dotnetRef) {
      if (dotnetRef && !subscribers.includes(dotnetRef)) {
        subscribers.push(dotnetRef);
      }
    },

    unsubscribe() {
      subscribers = [];
    },
  };
})();
