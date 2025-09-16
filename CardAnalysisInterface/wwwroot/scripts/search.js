// wwwroot/js/search.js

// Store a reference to the global keydown handler to allow for its removal.
let globalKeydownHandler;

function initializeSearchBar(element, dotNetObjectReference) {
    // --- Debounced Input Handler ---
    let debounceTimeout;
    const inputHandler = (e) => {
        clearTimeout(debounceTimeout);
        debounceTimeout = setTimeout(() => {
            dotNetObjectReference.invokeMethodAsync('UpdateSearchTerm', e.target.value);
        }, 300); // 300ms debounce interval
    };
    element.addEventListener('input', inputHandler);

    // Store the handler on the element for later removal during disposal.
    element.inputHandler = inputHandler;
    element.debounceTimeout = debounceTimeout;


    // --- Global Keydown Handler for Shortcuts ---
    globalKeydownHandler = (e) => {
        // Ctrl+F to focus the search bar
        if ((e.ctrlKey && e.key === 'f') || (e.ctrlKey && e.key === ',')) {
            e.preventDefault();
            element.focus();
            element.select();
        }
        // 3. Escape key to clear the search
        else if (e.key === 'Escape') {
            e.preventDefault();
            dotNetObjectReference.invokeMethodAsync('ClearSearch');
        }
    };
    document.addEventListener('keydown', globalKeydownHandler);
}

function disposeSearchBar(element) {
    // Remove the global keydown listener
    if (globalKeydownHandler) {
        document.removeEventListener('keydown', globalKeydownHandler);
        globalKeydownHandler = null; // Clear reference
    }

    // Remove the specific input listener from the search bar element
    if (element && element.inputHandler) {
        element.removeEventListener('input', element.inputHandler);
    }

    // Clear any pending debounce timers to prevent memory leaks
    if (element && element.debounceTimeout) {
        clearTimeout(element.debounceTimeout);
    }
}