// Regex Editor Dialog

function renderHighlights(containerId, matches) {
    const container = document.getElementById(containerId);
    if (!container) return;

    let highlightLayer = container.querySelector('.highlight-layer');
    if (!highlightLayer) {
        highlightLayer = document.createElement('div');
        highlightLayer.className = 'highlight-layer';
        container.appendChild(highlightLayer);
    }

    highlightLayer.innerHTML = '';

    if (!matches || matches.length === 0) {
        return;
    }

    function findNodeAndOffset(parent, characterIndex) {
        const walker = document.createTreeWalker(parent, NodeFilter.SHOW_TEXT);
        let remainingOffset = characterIndex;
        let currentNode;
        while (currentNode = walker.nextNode()) {
            if (remainingOffset <= currentNode.length) {
                return { node: currentNode, offset: remainingOffset };
            }
            remainingOffset -= currentNode.length;
        }
        return null;
    }

    for (const match of matches) {
        const startIndex = match.index;
        const endIndex = startIndex + match.length;

        const range = document.createRange();
        const start = findNodeAndOffset(container, startIndex);
        const end = findNodeAndOffset(container, endIndex);

        if (start && end) {
            range.setStart(start.node, start.offset);
            range.setEnd(end.node, end.offset);

            const rects = range.getClientRects();
            const containerRect = container.getBoundingClientRect();

            for (const rect of rects) {
                const highlightEl = document.createElement('div');
                highlightEl.className = 'preview-highlight';
                highlightEl.style.top = `${rect.top - containerRect.top}px`;
                highlightEl.style.left = `${rect.left - containerRect.left}px`;
                highlightEl.style.width = `${rect.width}px`;
                highlightEl.style.height = `${rect.height}px`;
                highlightLayer.appendChild(highlightEl);
            }
        }
    }
}

function registerDialogKeyListener(dotnetHelper) {
    const keydownHandler = (e) => {
        if (e.key === 'Escape') {
            dotnetHelper.invokeMethodAsync('HandleEscapeKeyPress');
        }
    };
    document.addEventListener('keydown', keydownHandler);
    window.dialogKeyListener = keydownHandler;
}

function disposeDialogKeyListener() {
    if (window.dialogKeyListener) {
        document.removeEventListener('keydown', window.dialogKeyListener);
        delete window.dialogKeyListener;
    }
}

function focusElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}

function scrollToElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ block: 'nearest' });
    }
}

// --- NEW/UPDATED FUNCTIONS FOR HIGHLIGHT FIX ---

function setKeyboardNavigating(isNavigating) {
    const dropdown = document.getElementById('autocomplete-dropdown-list');
    if (dropdown) {
        if (isNavigating) {
            dropdown.classList.add('keyboard-navigating');
        } else {
            dropdown.classList.remove('keyboard-navigating');
        }
    }
}

function initializeAutocompleteInteraction(dropdownId) {
    document.body.addEventListener('mousemove', (e) => {
        const dropdown = document.getElementById(dropdownId);
        if (!dropdown || !dropdown.classList.contains('keyboard-navigating')) {
            return;
        }

        if (dropdown.contains(e.target)) {
            setKeyboardNavigating(false);
            const activeItem = dropdown.querySelector('.autocomplete-item.selected');
            if (activeItem) {
                // In Blazor, it's safer to let the component manage its state,
                // so we won't manually remove the 'selected' class here.
                // The hover effect is now disabled by CSS, which is the main goal.
                // When Blazor re-renders on the next state change (e.g., arrow key),
                // the 'selected' class will be correctly reassigned anyway.
            }
        }
    });
}