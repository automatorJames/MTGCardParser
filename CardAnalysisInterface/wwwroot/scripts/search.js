// wwwroot/js/search.js

let searchInput;
let keydownHandler;
let inputHandler;
let debounceTimeout;

function initializeSearchBar(element, dotNetObjectReference) {
    searchInput = element;

    // --- Ctrl+F Handler ---
    keydownHandler = (e) => {
        if (e.ctrlKey && e.key === 'f' || e.ctrlKey && e.key === ',') {
            e.preventDefault();
            searchInput.focus();
            searchInput.select();
        }
    };
    document.addEventListener('keydown', keydownHandler);

    // --- Debounced Input Handler ---
    inputHandler = (e) => {
        // Clear any existing timer on each keystroke
        clearTimeout(debounceTimeout);

        // Set a new timer to call back to C# after 300ms of inactivity
        debounceTimeout = setTimeout(() => {
            dotNetObjectReference.invokeMethodAsync('UpdateSearchTerm', e.target.value);
        }, 300); // 300ms debounce interval
    };
    searchInput.addEventListener('input', inputHandler);
}

function disposeSearchBar() {
    // Remove the global keydown listener
    if (keydownHandler) {
        document.removeEventListener('keydown', keydownHandler);
    }
    // Remove the input listener from the search bar
    if (searchInput && inputHandler) {
        searchInput.removeEventListener('input', inputHandler);
    }
    // IMPORTANT: Clear any pending debounce timers to prevent memory leaks on dispose
    if (debounceTimeout) {
        clearTimeout(debounceTimeout);
    }
}