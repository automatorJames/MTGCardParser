// span-tree-manager.ts
import { setupGlobalEventHandlers, wordTreeObservers } from "./span-tree-event-handler.js";
import { orchestrateWordTreeRender } from "./span-tree-orchestrator.js";
/**
 * Processes raw span data from the server, augmenting it for efficient client-side use.
 * This converts key arrays into Sets for faster lookups.
 * @param rawSpan The raw analysis data for a span.
 * @returns The processed and augmented span data.
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
// === Blazor Interop Functions ===
/**
 * Clears all rendered word trees and displays loading spinners.
 * Also cleans up observers for any trees that are being removed.
 * @param count The number of word tree containers to prepare.
 */
export function clearAllTreesAndShowSpinners(count) {
    setupGlobalEventHandlers();
    // Disconnect observers for trees that no longer exist
    for (const id of wordTreeObservers.keys()) {
        const index = parseInt(id.split('-').pop() || '-1');
        if (index >= count) {
            const observerData = wordTreeObservers.get(id);
            if (observerData) {
                observerData.observer.disconnect();
                if (observerData.animationFrameId)
                    cancelAnimationFrame(observerData.animationFrameId);
                wordTreeObservers.delete(id);
            }
        }
    }
    // Reset containers and show spinners
    for (let i = 0; i < count; i++) {
        const containerId = `word-tree-container-${i}`;
        const spinnerId = `spinner-${i}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);
        if (container) {
            container.innerHTML = '';
            container.style.height = '';
        }
        if (spinner) {
            spinner.style.display = 'block';
        }
    }
}
/**
 * Renders a word tree for each provided span object.
 * @param spans An array of raw span data from the server.
 */
export function renderAllTrees(spans) {
    spans.forEach((rawSpan, index) => {
        const containerId = `word-tree-container-${index}`;
        const spinnerId = `spinner-${index}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);
        if (!container)
            return;
        const card = container.closest('.span-trees-card');
        if (card) {
            card.__data = processSpanForClient(rawSpan);
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
    });
}
// Expose the Blazor interop functions to the global scope for easy access.
window.clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
window.renderAllTrees = renderAllTrees;
//# sourceMappingURL=span-tree-manager.js.map