// document-lines.js

// --- DATA-PATH HIERARCHICAL HOVER HIGHLIGHTING ---

let mouseoverHandler;
let mouseleaveHandler;
let lastHoveredElement = null;

const highlightActiveClass = 'highlight-active';
const muteActiveClass = 'mute-active';
const dataPathSelector = '[data-path]';
const boundaryClass = 'match-boundary';

function initDocumentCaptureHover() {
    const mainContent = document.getElementById('document-lines');
    if (!mainContent) {
        return;
    }

    const clearClasses = () => {
        const activeElements = document.querySelectorAll(`.${highlightActiveClass}, .${muteActiveClass}`);
        activeElements.forEach(el => {
            el.classList.remove(highlightActiveClass);
            el.classList.remove(muteActiveClass);
        });
    };

    mouseoverHandler = (event) => {
        const hoveredElement = event.target.closest(dataPathSelector);

        // If the mouse is moving within the same element (or same empty space), do nothing.
        if (hoveredElement === lastHoveredElement) return;

        // The hover target has changed. Update the reference and clear previous state.
        lastHoveredElement = hoveredElement;
        clearClasses();

        // If we moved into empty space (no data-path), we stop here after clearing.
        if (!hoveredElement) return;

        const hoveredPath = hoveredElement.dataset.path;
        if (!hoveredPath) return;

        const boundary = hoveredElement.closest('.' + boundaryClass);
        if (!boundary) return;

        // --- PHASE 1: COLLECT ---
        const pathsToHighlight = new Set();
        let currentElement = hoveredElement;

        while (currentElement && currentElement !== boundary.parentElement) {
            const currentPath = currentElement.dataset.path;
            if (currentPath) {
                pathsToHighlight.add(currentPath);
            }
            currentElement = currentElement.parentElement;
        }

        // --- PHASE 2: DISTRIBUTE ---
        if (pathsToHighlight.size > 0) {
            const allPathElementsInBoundary = boundary.querySelectorAll(dataPathSelector);
            allPathElementsInBoundary.forEach(el => {
                if (pathsToHighlight.has(el.dataset.path)) {
                    el.classList.add(highlightActiveClass);
                } else {
                    el.classList.add(muteActiveClass);
                }
            });
        }
    };

    mouseleaveHandler = () => {
        lastHoveredElement = null;
        clearClasses();
    };

    mainContent.addEventListener('mouseover', mouseoverHandler);
    mainContent.addEventListener('mouseleave', mouseleaveHandler);
}

function disposeDocumentCaptureHover() {
    const mainContent = document.getElementById('document-lines');
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}

function flashLineHighlight(dataPath) {
    // Select the specific container by class and data-path
    const selector = `.line-container-parent[data-path="${dataPath}"]`;
    const element = document.querySelector(selector);

    if (element) {
        // Remove the class first in case it's already there (re-trigger)
        element.classList.remove('flash-highlight');

        // Force a reflow to ensure the browser notices the class was removed
        void element.offsetWidth;

        element.classList.add('flash-highlight');

        // Clean up the class after the animation finishes
        element.addEventListener('animationend', () => {
            element.classList.remove('flash-highlight');
        }, { once: true });
    }
}