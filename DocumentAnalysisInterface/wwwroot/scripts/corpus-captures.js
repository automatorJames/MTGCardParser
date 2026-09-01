// corpus-captures.js

// --- DATA-PATH HIERARCHICAL HOVER HIGHLIGHTING ---

let mouseoverHandler;
let mouseleaveHandler;
let lastHoveredElement = null;

const highlightActiveClass = 'highlight-active';
const muteActiveClass = 'mute-active';
const dataPathSelector = '[data-path]';
const boundaryClass = 'match-boundary';

/// Whether `path` is one of `collectedPaths`, or an ancestor of one of them. A data-path is the
/// underscore-joined chain of named groups from the line's root down to one capture, so a path that
/// is another's prefix at an underscore boundary IS its ancestor - and the DOM ancestor walk in
/// PHASE 1 doesn't find every such ancestor on its own. Most of the time it does (a capture's span
/// really is nested inside its parent's span), but the property table's own multi-part header
/// renders the collapsed chain that led to a capture as a flat row of sibling parts - "Buff »
/// Transformed Type : Card Type" - each carrying its own path. Matching by lineage rather than by
/// DOM position is what lets hovering the middle part light up itself and everything above it while
/// leaving the more specific parts to its right muted.
function isSelfOrAncestorPath(path, collectedPaths) {
    if (collectedPaths.has(path)) return true;

    for (const collected of collectedPaths) {
        if (collected.startsWith(path + '_')) return true;
    }

    return false;
}

function initDocumentCaptureHover() {
    const mainContent = document.getElementById('corpus-captures');
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
                if (isSelfOrAncestorPath(el.dataset.path, pathsToHighlight)) {
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
    const mainContent = document.getElementById('corpus-captures');
    if (mainContent && mouseoverHandler && mouseleaveHandler) {
        mainContent.removeEventListener('mouseover', mouseoverHandler);
        mainContent.removeEventListener('mouseleave', mouseleaveHandler);
    }
}

// --- ECHO GROUP HOVER/CLICK — Y-DISTANCE DISAMBIGUATION ---
// A single echo's underline is drawn as several separate per-word/per-space segments, one <span>
// per lane nested inside the next (so it can wrap and stack like normal text, and carry several
// simultaneous underlines on one word). That nesting turns out to defeat the browser's native
// hit-testing for our purposes: for stacked inline elements sharing one line, elementFromPoint
// (and therefore native :hover/click target resolution) always resolves to whichever is nested
// deepest, at *every* Y within the shared line — never the one geometrically under the cursor.
// Confirmed by sampling elementFromPoint down a real 3-lane stack: same element at every offset.
//
// So selection can't be native. Instead: on every mousemove/click, take whichever lane the browser
// DID resolve to (always reachable — it's some element in the right stack, just not necessarily
// the right lane) and walk its own .echo-underline ancestor chain — each ancestor is one more lane
// at this exact word — comparing each one's own getBoundingClientRect() (a real per-element
// measurement, unaffected by the hit-testing quirk) against the pointer's Y. Closest bottom wins.

let echoMouseMoveHandler;
let echoMouseleaveHandler;
let echoClickHandler;
let lastHoveredEchoElement = null;

const echoHoverActiveClass = 'echo-hover-active';
const echoKeySelector = '[data-echo-key]';
const echoResolvedFlag = 'echoResolvedClick';

/// Walks from `hitElement` up through its own chain of .echo-underline ancestors (every lane
/// stacked at this word) and returns whichever one's own bottom edge is closest to clientY.
function resolveEchoLane(hitElement, clientY) {
    let best = hitElement;
    let bestDistance = Infinity;
    let current = hitElement;

    while (current && current.matches && current.matches(echoKeySelector)) {
        const rect = current.getBoundingClientRect();
        const distance = Math.abs(clientY - rect.bottom);
        if (distance < bestDistance) {
            bestDistance = distance;
            best = current;
        }
        current = current.parentElement;
    }

    return best;
}

function initEchoHover() {
    const mainContent = document.getElementById('corpus-captures');
    if (!mainContent) {
        return;
    }

    const clearEchoHighlight = () => {
        document.querySelectorAll(`.${echoHoverActiveClass}`).forEach(el => {
            el.classList.remove(echoHoverActiveClass);
        });
    };

    echoMouseMoveHandler = (event) => {
        const hitElement = event.target.closest(echoKeySelector);

        if (!hitElement) {
            if (lastHoveredEchoElement === null) return;
            lastHoveredEchoElement = null;
            clearEchoHighlight();
            return;
        }

        const resolved = resolveEchoLane(hitElement, event.clientY);
        if (resolved === lastHoveredEchoElement) return;

        lastHoveredEchoElement = resolved;
        clearEchoHighlight();

        const container = resolved.closest('.echo-container');
        const key = resolved.dataset.echoKey;
        if (!container || !key) return;

        container.querySelectorAll(`[data-echo-key="${key}"]`).forEach(el => {
            el.classList.add(echoHoverActiveClass);
        });
    };

    echoMouseleaveHandler = () => {
        lastHoveredEchoElement = null;
        clearEchoHighlight();
    };

    // Capture phase, so this runs (and can redirect) before Blazor's own delegated click
    // handling sees the event. Without redirecting, a click always lands on whichever lane is
    // nested deepest at that word, same as hover — see the comment above.
    echoClickHandler = (event) => {
        const hitElement = event.target.closest(echoKeySelector);
        if (!hitElement) return;

        if (hitElement.dataset[echoResolvedFlag]) {
            delete hitElement.dataset[echoResolvedFlag];
            return; // our own re-dispatched click, let it through to Blazor untouched
        }

        const resolved = resolveEchoLane(hitElement, event.clientY);
        if (resolved === hitElement) return; // the natively-hit lane was already the right one

        event.preventDefault();
        event.stopPropagation();
        resolved.dataset[echoResolvedFlag] = '1';
        resolved.click();
    };

    mainContent.addEventListener('mousemove', echoMouseMoveHandler);
    mainContent.addEventListener('mouseleave', echoMouseleaveHandler);
    mainContent.addEventListener('click', echoClickHandler, true);
}

function disposeEchoHover() {
    const mainContent = document.getElementById('corpus-captures');
    if (!mainContent) return;
    if (echoMouseMoveHandler) mainContent.removeEventListener('mousemove', echoMouseMoveHandler);
    if (echoMouseleaveHandler) mainContent.removeEventListener('mouseleave', echoMouseleaveHandler);
    if (echoClickHandler) mainContent.removeEventListener('click', echoClickHandler, true);
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