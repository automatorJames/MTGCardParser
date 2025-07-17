// --- DATA-PATH HIERARCHICAL HOVER HIGHLIGHTING ---

// These variables will hold our event handlers so they can be removed later,
// crucial for preventing memory leaks in a Single Page Application (SPA).
let mouseoverHandler;
let mouseleaveHandler;

const highlightActiveClass = 'highlight-active';
const muteActiveClass = 'mute-active'; // New class for muting
const dataPathSelector = '[data-path]';
const boundaryClass = 'match-boundary';

function initCardCaptureHover() {
    const mainContent = document.getElementById('card-analysis');
    if (!mainContent) {
        return;
    }

    // A handler to find and remove all existing highlights and mutes from the page.
    const clearClasses = () => {
        // Query for all elements with either class for a comprehensive cleanup.
        const activeElements = document.querySelectorAll(`.${highlightActiveClass}, .${muteActiveClass}`);
        activeElements.forEach(el => {
            el.classList.remove(highlightActiveClass);
            el.classList.remove(muteActiveClass);
        });
    };

    // This is the core logic, which executes on every mouseover event.
    mouseoverHandler = (event) => {
        // Always start with a clean slate by clearing previous classes.
        clearClasses();

        // Find the element with a data-path that the user is actually hovering over.
        const hoveredElement = event.target.closest(dataPathSelector);
        if (!hoveredElement) return;

        const hoveredPath = hoveredElement.dataset.path;
        if (!hoveredPath) return;

        // Determine the boundary for the current interaction.
        const boundary = hoveredElement.closest('.' + boundaryClass);
        if (!boundary) return;


        // --- EFFICIENT HIGHLIGHTING & MUTING ALGORITHM ---
        // This solution avoids performance issues by splitting the work into two phases:
        // 1. A fast, targeted "Collect" phase that traverses UP the DOM tree.
        // 2. A single "Distribute" phase that scans only within the boundary for matches.
        // This is vastly more performant than doing complex comparisons on every element in the DOM.

        // --- PHASE 1: COLLECT ---
        // We travel up from the hovered element, collecting the `data-path` of all valid
        // ancestors into a Set. A Set provides highly efficient, near-instant lookups.
        const pathsToHighlight = new Set();
        let currentElement = hoveredElement;

        while (currentElement && currentElement !== boundary.parentElement) {
            const currentPath = currentElement.dataset.path;

            // An ancestor is valid if its path is a prefix of the hovered path.
            // This elegantly identifies all hierarchical parents.
            if (currentPath && hoveredPath.startsWith(currentPath)) {
                pathsToHighlight.add(currentPath);
            }

            currentElement = currentElement.parentElement;
        }


        // --- PHASE 2: DISTRIBUTE ---
        // Now, with a small and efficient Set of paths to find, we do one single
        // scan of all elements with a `data-path` *within the boundary*.
        // For each element, we check if its path exists in our Set.
        if (pathsToHighlight.size > 0) {
            const allPathElementsInBoundary = boundary.querySelectorAll(dataPathSelector);
            allPathElementsInBoundary.forEach(el => {
                if (pathsToHighlight.has(el.dataset.path)) {
                    // If the path matches, add the highlight class.
                    el.classList.add(highlightActiveClass);
                } else {
                    // Otherwise, add the mute class.
                    el.classList.add(muteActiveClass);
                }
            });
        }
    };

    // The mouseleave event should always fire immediately to clear classes.
    mouseleaveHandler = () => clearClasses();

    // Attach the finalized event listeners.
    mainContent.addEventListener('mouseover', mouseoverHandler);
    mainContent.addEventListener('mouseleave', mouseleaveHandler);
}

function disposeCardCaptureHover() {
    const mainContent = document.getElementById('card-analysis');
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}

// Regex Editor Dialog

// This function receives an array of matches from Blazor and renders them.
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

    // --- REVISED and ROBUST findNodeAndOffset function ---
    // This function uses a TreeWalker to reliably find the text node and offset
    // for a given character index, correctly handling nested <span> elements.
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
        return null; // Index is out of bounds
    }

    for (const match of matches) {
        const startIndex = match.index;
        const endIndex = startIndex + match.length;

        const range = document.createRange();
        const start = findNodeAndOffset(container, startIndex);
        const end = findNodeAndOffset(container, endIndex);

        // Ensure both start and end points were found before creating a range.
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

// This function registers a global keydown event listener.
function registerDialogKeyListener(dotnetHelper) {
    // We define the handler function inside so we can reference it later to remove it.
    const keydownHandler = (e) => {
        if (e.key === 'Escape') {
            // Call the specified method on our .NET component instance.
            dotnetHelper.invokeMethodAsync('HandleEscapeKeyPress');
        }
    };

    document.addEventListener('keydown', keydownHandler);

    // Store the handler on a global object so we can find it to dispose of it.
    // This prevents adding multiple listeners if the dialog is opened/closed quickly.
    window.dialogKeyListener = keydownHandler;
}

// This function cleans up the event listener to prevent memory leaks.
function disposeDialogKeyListener() {
    if (window.dialogKeyListener) {
        document.removeEventListener('keydown', window.dialogKeyListener);
        delete window.dialogKeyListener;
    }
}

// This function finds an element by its ID and sets focus on it.
function focusElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
    }
}