// span-tree-manager.ts

import { AnalyzedSpan, ProcessedAnalyzedSpan, AdjacencyNode, CardElement } from "./models.js";
import { setupGlobalEventHandlers, wordTreeObservers } from "./span-tree-event-handler.js";
import { orchestrateWordTreeRender } from "./span-tree-orchestrator.js";

/**
 * Processes raw span data from the server, augmenting it for efficient client-side use.
 * This converts key arrays into Sets for faster lookups.
 * @param rawSpan The raw analysis data for a span.
 * @returns The processed and augmented span data.
 */
function processSpanForClient(rawSpan: AnalyzedSpan): ProcessedAnalyzedSpan {
    const traverseAndAugmentNodes = (nodes: AdjacencyNode[]): void => {
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
 * Also cleans up observers for any trees that are being removed from the display.
 * @param count The number of word tree containers to prepare.
 */
export function clearAllTreesAndShowSpinners(count: number): void {
    setupGlobalEventHandlers();

    // Disconnect observers for trees that are no longer present in the new layout
    for (const id of wordTreeObservers.keys()) {
        const index = parseInt(id.split('-').pop() || '-1');
        if (index >= count) {
            const observerData = wordTreeObservers.get(id);
            if (observerData) {
                observerData.observer.disconnect();
                if (observerData.animationFrameId) {
                    cancelAnimationFrame(observerData.animationFrameId);
                }
                wordTreeObservers.delete(id);
            }
        }
    }

    // Reset the state of the remaining containers and show their spinners
    for (let i = 0; i < count; i++) {
        const containerId = `word-tree-container-${i}`;
        const spinnerId = `spinner-${i}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);

        if (container) {
            container.innerHTML = ''; // Clear any previous SVG
            container.style.height = ''; // Reset height
        }
        if (spinner) {
            spinner.style.display = 'block';
        }
    }
}

/**
 * Renders a word tree for each provided span object from the server.
 * This is the main entry point for drawing the visualization.
 * @param spans An array of raw span data from the server.
 */
export function renderAllTrees(spans: AnalyzedSpan[]): void {
    spans.forEach((rawSpan, index) => {
        const containerId = `word-tree-container-${index}`;
        const spinnerId = `spinner-${index}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);

        if (!container) {
            console.error(`Container with id "${containerId}" not found.`);
            return;
        }

        const card = container.closest<CardElement>('.span-trees-card');
        if (card) {
            // Process and attach the data to the card element for easy access by event handlers
            card.__data = processSpanForClient(rawSpan);
        }

        // Set up a ResizeObserver for this container if it doesn't already have one
        if (!wordTreeObservers.has(containerId)) {
            const resizeObserver = new ResizeObserver(() => orchestrateWordTreeRender(container));
            resizeObserver.observe(container);
            wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });
        }

        const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
        container.appendChild(svg);

        // Kick off the full rendering process
        orchestrateWordTreeRender(container);

        if (spinner) {
            spinner.style.display = 'none';
        }
    });
}

// Expose the Blazor interop functions to the global window object for easy access.
(window as any).clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
(window as any).renderAllTrees = renderAllTrees;