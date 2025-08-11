// span-tree-manager.ts
import { setupGlobalEventHandlers, wordTreeObservers } from "./span-tree-event-handler.js";
import { orchestrateWordTreeRender } from "./span-tree-orchestrator.js";
// --- Virtualization Configuration ---
const virtualizationConfig = {
    /** The number of trees to render on the initial load. */
    initialBatchSize: 5,
    /** The number of additional trees to render each time the user scrolls near the bottom. */
    loadMoreBatchSize: 10,
    /** How close (in pixels) the user must be to the bottom of the rendered content
     *  to trigger loading the next batch. A larger value means loading sooner. */
    scrollThreshold: 800
};
// --- Module-level State for Virtualization ---
let fullDataset = [];
let nextItemToRender = 0;
let isLoadingMore = false;
let scrollListenerAttached = false;
/**
 * Processes raw span data from the server, augmenting it for efficient client-side use.
 * This converts key arrays into Sets for faster lookups.
 */
function processSpanForClient(rawSpan) {
    const traverseAndAugmentNodes = (nodes) => {
        for (const node of nodes) {
            node.sourceKeysSet = new Set(node.sourceOccurrenceKeys);
            if (node.children) {
                traverseAndAugmentNodes(node.children);
            }
        }
    };
    traverseAndAugmentNodes(rawSpan.precedingAdjacencies);
    traverseAndAugmentNodes(rawSpan.followingAdjacencies);
    return {
        ...rawSpan,
        keyToPaletteMap: new Map(Object.entries(rawSpan.keyToPaletteMap)),
        allKeys: new Set(rawSpan.allKeys),
    };
}
/**
 * Renders a single word tree into its designated container.
 * @param spanData The raw data for the tree to render.
 * @param index The global index of the tree, used to find the correct container.
 */
function renderSingleTree(spanData, index) {
    const containerId = `word-tree-container-${index}`;
    const spinnerId = `spinner-${index}`;
    const container = document.getElementById(containerId);
    const spinner = document.getElementById(spinnerId);
    if (!container)
        return;
    const card = container.closest('.span-trees-card');
    if (card) {
        card.__data = processSpanForClient(spanData);
    }
    if (!wordTreeObservers.has(containerId)) {
        const resizeObserver = new ResizeObserver(() => orchestrateWordTreeRender(container));
        resizeObserver.observe(container);
        wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });
    }
    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    container.appendChild(svg);
    orchestrateWordTreeRender(container);
    if (spinner) {
        spinner.style.display = 'none';
    }
}
/**
 * Renders the next available batch of word trees from the full dataset.
 */
function renderNextBatch() {
    if (isLoadingMore || nextItemToRender >= fullDataset.length) {
        return; // Either already loading or all items have been rendered
    }
    isLoadingMore = true;
    const batchSize = nextItemToRender === 0
        ? virtualizationConfig.initialBatchSize
        : virtualizationConfig.loadMoreBatchSize;
    const batchEnd = Math.min(nextItemToRender + batchSize, fullDataset.length);
    for (let i = nextItemToRender; i < batchEnd; i++) {
        renderSingleTree(fullDataset[i], i);
    }
    nextItemToRender = batchEnd;
    isLoadingMore = false;
}
/**
 * Checks the user's scroll position and triggers rendering the next batch if needed.
 */
function handleScroll() {
    if (isLoadingMore || nextItemToRender >= fullDataset.length) {
        return;
    }
    // Find the last rendered element to check its position
    const lastRenderedIndex = nextItemToRender - 1;
    if (lastRenderedIndex < 0)
        return;
    const lastContainer = document.getElementById(`word-tree-container-${lastRenderedIndex}`);
    if (!lastContainer)
        return;
    const rect = lastContainer.getBoundingClientRect();
    // If the bottom of the last rendered element is within the viewport plus the threshold, load more.
    if (rect.bottom < window.innerHeight + virtualizationConfig.scrollThreshold) {
        // Use requestAnimationFrame to ensure the render call happens smoothly
        requestAnimationFrame(renderNextBatch);
    }
}
// === Blazor Interop Functions ===
/**
 * Clears all word tree containers and displays loading spinners in their place.
 * This prepares the DOM for a fresh render.
 */
export function clearAllTreesAndShowSpinners(count) {
    setupGlobalEventHandlers();
    for (const id of wordTreeObservers.keys()) {
        const observerData = wordTreeObservers.get(id);
        if (observerData) {
            observerData.observer.disconnect();
            if (observerData.animationFrameId) {
                cancelAnimationFrame(observerData.animationFrameId);
            }
        }
    }
    wordTreeObservers.clear();
    for (let i = 0; i < count; i++) {
        const containerId = `word-tree-container-${i}`;
        const spinnerId = `spinner-${i}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);
        if (container) {
            container.innerHTML = '';
            container.style.height = '300px'; // Give placeholder a default height for the spinner
        }
        if (spinner) {
            spinner.style.display = 'block';
        }
    }
}
/**
 * Initializes the virtualized rendering process for a full set of spans.
 * It renders the first batch and sets up a scroll listener to render more on demand.
 * @param spans The complete array of raw span data from the server.
 */
export function renderAllTrees(spans) {
    // 1. Reset state and store the full dataset
    isLoadingMore = false;
    nextItemToRender = 0;
    fullDataset = spans;
    // 2. Prepare all DOM containers for rendering
    clearAllTreesAndShowSpinners(spans.length);
    // 3. Render the initial batch of trees
    renderNextBatch();
    // 4. Attach the scroll listener if it hasn't been already
    if (!scrollListenerAttached) {
        window.addEventListener('scroll', handleScroll, { passive: true });
        scrollListenerAttached = true;
    }
}
// Expose the Blazor interop functions to the global window object.
window.clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
window.renderAllTrees = renderAllTrees;
//# sourceMappingURL=span-tree-manager.js.map