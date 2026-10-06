// wwwroot/js/search.js

// Store a reference to the global keydown handler to allow for its removal.
let globalKeydownHandler;

// The input owns its own text: Blazor never writes it back (a term arriving from the server after the
// debounce is already older than what's in the box, and writing it would erase the keystrokes typed since).
// The only change from outside is clearing, which goes through clearSearchBar.
function initializeSearchBar(element, dotNetObjectReference) {
    // --- Debounced Input Handler ---
    const inputHandler = () => {
        clearTimeout(element.debounceTimeout);
        element.debounceTimeout = setTimeout(() => {
            dotNetObjectReference.invokeMethodAsync('UpdateSearchTerm', element.value);
        }, 300); // 300ms debounce interval
    };
    element.addEventListener('input', inputHandler);

    // Store the handler on the element for later removal during disposal.
    element.inputHandler = inputHandler;


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

// Empties the box and drops any term still waiting out its debounce, so it can't arrive after the clear.
function clearSearchBar(element) {
    if (!element)
        return;

    clearTimeout(element.debounceTimeout);
    element.value = '';
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
    if (element) {
        clearTimeout(element.debounceTimeout);
    }
}
