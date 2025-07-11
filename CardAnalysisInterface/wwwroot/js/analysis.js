// --- CLICK TO COPY LOGIC ---
// This part remains unchanged and works fine as it is.
document.addEventListener('DOMContentLoaded', () => {
    document.body.addEventListener('click', (event) => {
        let target = event.target.closest('pre, td');
        if (!target) return;
        if (document.body.classList.contains('page-variable-capture') && !target.classList.contains('full-original-text')) return;
        let textToCopy = (target.tagName === 'TD' && target.hasAttribute('data-original-text')) ? target.getAttribute('data-original-text') : target.innerText.trim();
        if (textToCopy) {
            if (!event.shiftKey) textToCopy = textToCopy.toLowerCase();
            navigator.clipboard.writeText(textToCopy).then(() => showCopyFeedback(event.clientX, event.clientY)).catch(err => console.error('Failed to copy text: ', err));
        }
    });

    let feedbackDiv = null, feedbackTimeout = null;
    function showCopyFeedback(x, y) {
        if (!feedbackDiv) {
            feedbackDiv = document.createElement('div');
            feedbackDiv.className = 'copy-feedback';
            feedbackDiv.textContent = 'Copied';
            document.body.appendChild(feedbackDiv);
        }
        clearTimeout(feedbackTimeout);
        feedbackDiv.style.left = `${x + 15}px`;
        feedbackDiv.style.top = `${y + 15}px`;
        feedbackDiv.style.opacity = '1';
        feedbackTimeout = setTimeout(() => { feedbackDiv.style.opacity = '0'; }, 1200);
    }
});


// --- DATA-PATH HIERARCHICAL HOVER HIGHLIGHTING ---

// These variables will hold our event handlers so they can be removed later,
// crucial for preventing memory leaks in a Single Page Application (SPA).
let mouseoverHandler;
let mouseleaveHandler;

const highlightActiveClass = 'highlight-active';
const dataPathSelector = '[data-path]';
const boundaryClass = 'match-boundary';

function initCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (!mainContent) {
        return;
    }

    // A simple handler to find and remove all existing highlights from the page.
    const clearHighlights = () => {
        // Querying by the class is more direct than storing a list of highlighted elements.
        const highlightedElements = document.querySelectorAll('.' + highlightActiveClass);
        highlightedElements.forEach(el => {
            el.classList.remove(highlightActiveClass);
        });
    };

    // This is the core logic, which executes on every mouseover event.
    mouseoverHandler = (event) => {
        // Always start with a clean slate by clearing previous highlights.
        clearHighlights();

        // Find the element with a data-path that the user is actually hovering over.
        const hoveredElement = event.target.closest(dataPathSelector);
        if (!hoveredElement) return;

        const hoveredPath = hoveredElement.dataset.path;
        if (!hoveredPath) return;


        // --- EFFICIENT HIGHLIGHTING ALGORITHM ---
        // This brilliant solution avoids performance issues by splitting the work into two phases:
        // 1. A fast, targeted "Collect" phase that traverses UP the DOM tree.
        // 2. A single "Distribute" phase that scans the document for exact matches.
        // This is vastly more performant than doing complex comparisons on every element in the DOM.

        // --- PHASE 1: COLLECT ---
        // We travel up from the hovered element, collecting the `data-path` of all valid
        // ancestors into a Set. A Set provides highly efficient, near-instant lookups.
        const pathsToHighlight = new Set();
        let currentElement = hoveredElement;

        while (currentElement) {
            const currentPath = currentElement.dataset.path;

            // An ancestor is valid if its path is a prefix of the hovered path.
            // This elegantly identifies all hierarchical parents.
            if (currentPath && hoveredPath.startsWith(currentPath)) {
                pathsToHighlight.add(currentPath);
            }

            // Stop traversing upwards once we hit a designated boundary parent.
            // This contains the search to a relevant component area.
            if (currentElement.classList.contains(boundaryClass)) {
                break;
            }

            currentElement = currentElement.parentElement;
        }

        // --- PHASE 2: DISTRIBUTE ---
        // Now, with a small and efficient Set of paths to find, we do one single
        // scan of the document. For each element with a data-path, we check if
        // its path exists in our Set. This is the key to the algorithm's speed.
        if (pathsToHighlight.size > 0) {
            const allPathElements = document.querySelectorAll(dataPathSelector);
            allPathElements.forEach(el => {
                if (pathsToHighlight.has(el.dataset.path)) {
                    el.classList.add(highlightActiveClass);
                }
            });
        }
    };

    // The mouseleave event should always fire immediately to clear highlights.
    mouseleaveHandler = () => clearHighlights();

    // Attach the finalized event listeners.
    mainContent.addEventListener('mouseover', mouseoverHandler);
    mainContent.addEventListener('mouseleave', mouseleaveHandler);
}

function disposeCardCaptureHover() {
    const mainContent = document.getElementById('main-content');
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}