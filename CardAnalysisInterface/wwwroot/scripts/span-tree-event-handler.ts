// span-tree-event-handler.ts

import { CardElement, WordTreeObserver } from "./models.js";
import { WordTree } from "./word-tree-animator.js";
import { createGradientStops } from "./word-tree-svg-drawer.js";

// This state is managed here but used by the span-tree-manager.
export const wordTreeObservers = new Map<string, WordTreeObserver>();

const globalEventState = {
    initialized: false,
    // Keep track of the last hovered element to avoid redundant animations
    lastHovered: {
        card: null as CardElement | null,
        keys: new Set<string>()
    }
};

/**
 * A utility to check for deep equality between two Sets of strings.
 */
function areSetsEqual(setA: Set<string>, setB: Set<string>): boolean {
    if (setA.size !== setB.size) return false;
    for (const item of setA) {
        if (!setB.has(item)) return false;
    }
    return true;
}


/**
 * Finds the unique container ID for a given card element.
 */
function findContainerIdForCard(card: CardElement): string | null {
    const container = card.querySelector<HTMLElement>('.word-tree-body [id^="word-tree-container-"]');
    return container?.id || null;
}

/**
 * Highlights parts of the tree based on a set of filter keys (card names).
 */
function animateHighlightState(containerId: string, card: CardElement, filterKeys: Set<string>): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationController = wordTreeObservers.get(containerId);
    const processedData = card.__data;
    if (!svg || !animationController || !processedData || filterKeys.size === 0) return;

    const defs = svg.querySelector('defs');
    if (!defs) return;

    const { cardPalettes } = processedData;
    const gradientTransitionRatio = 0.1;
    const elementsToAnimate = new Map<HTMLElement, { start: number, end: number }>();

    svg.querySelectorAll<HTMLElement>('[data-source-keys]').forEach(element => {
        const elementKeys = JSON.parse(element.dataset.sourceKeys || '[]') as string[];
        const isHighlighted = elementKeys.some((key: string) => filterKeys.has(key));

        const baseLayer = element.querySelector<HTMLElement>('.base-layer') || element;
        const highlightOverlay = element.querySelector<HTMLElement>('.highlight-overlay');
        const nodeText = element.querySelector<HTMLElement>('.node-text');
        const targetOpacity = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;

        elementsToAnimate.set(baseLayer, { start: parseFloat(getComputedStyle(baseLayer).opacity), end: targetOpacity });
        if (highlightOverlay) elementsToAnimate.set(highlightOverlay, { start: parseFloat(getComputedStyle(highlightOverlay).opacity), end: isHighlighted ? 1 : 0 });
        if (nodeText) elementsToAnimate.set(nodeText, { start: parseFloat(getComputedStyle(nodeText).opacity), end: targetOpacity });

        // Dynamically update the gradient definition for the highlight overlay
        const idParts = element.id.split('-');
        const elementType = idParts[1];
        const elementId = idParts[idParts.length - 1];
        const keysForGradient = elementKeys.filter((key: string) => filterKeys.has(key));

        const highlightGradId = `grad-${elementType}-highlight-${containerId}-${elementId}`;
        const highlightGrad = defs.querySelector(`#${highlightGradId}`);
        if (highlightGrad) {
            highlightGrad.innerHTML = createGradientStops(keysForGradient, cardPalettes, 'hexSat', gradientTransitionRatio);
        }
    });

    WordTree.Animator.animateOpacity(elementsToAnimate, animationController);

    card.classList.add('highlight-active');
    // `filterKeys` is now the set of relevant card names. No parsing needed.
    const relevantCardNames = filterKeys;

    card.querySelectorAll<HTMLElement>('[data-card-name]').forEach(item => {
        const cardName = item.dataset.cardName || '';
        const isRelevant = relevantCardNames.has(cardName);
        item.classList.toggle('highlight', isRelevant);
        item.classList.toggle('lowlight', !isRelevant);
    });
}

/**
 * Resets a card's visual state to the default, removing all highlights.
 */
function animateResetState(containerId: string, card: CardElement): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationController = wordTreeObservers.get(containerId);
    if (!svg || !animationController) return;

    const elementsToAnimate = new Map<HTMLElement, { start: number; end: number }>();
    svg.querySelectorAll<HTMLElement>('.base-layer, .node-text, .highlight-overlay').forEach(element => {
        const isHighlight = element.classList.contains('highlight-overlay');
        elementsToAnimate.set(element, { start: parseFloat(getComputedStyle(element).opacity), end: isHighlight ? 0 : 1 });
    });
    WordTree.Animator.animateOpacity(elementsToAnimate, animationController);

    card.classList.remove('highlight-active');
    card.querySelectorAll<HTMLElement>('[data-card-name]').forEach(item => {
        item.classList.remove('highlight', 'lowlight');
    });
}

/**
 * Sets up a single, intelligent global event listener for hover interactions.
 * This handler manages state to determine when to highlight or reset the view.
 */
export function setupGlobalEventHandlers(): void {
    if (globalEventState.initialized) return;

    document.addEventListener('mouseover', (event: MouseEvent) => {
        const target = event.target as Element;
        const card = target.closest<CardElement>('.span-trees-card');

        // Case 1: Mouse is not over any card.
        // If we were previously hovering a card, reset it and clear the state.
        if (!card) {
            if (globalEventState.lastHovered.card) {
                const oldContainerId = findContainerIdForCard(globalEventState.lastHovered.card);
                if (oldContainerId) {
                    animateResetState(oldContainerId, globalEventState.lastHovered.card);
                }
                globalEventState.lastHovered = { card: null, keys: new Set() };
            }
            return;
        }

        // We are inside a card. Find its container.
        const containerId = findContainerIdForCard(card);
        if (!containerId) return;

        // Determine if the mouse is over an interactive element and get its keys (card names).
        const interactiveEl = target.closest<HTMLElement>('[data-source-keys], [data-card-name]');
        let currentKeys = new Set<string>();
        if (interactiveEl) {
            if (interactiveEl.dataset.sourceKeys) {
                // `sourceKeys` now directly contains the card names.
                currentKeys = new Set(JSON.parse(interactiveEl.dataset.sourceKeys));
            } else if (interactiveEl.dataset.cardName) {
                // If hovering a legend item, the key is simply the card name itself.
                const cardName = interactiveEl.dataset.cardName;
                if (cardName) {
                    currentKeys = new Set([cardName]);
                }
            }
        }

        // Case 2: The hover state has changed (different card or different keys).
        if (card !== globalEventState.lastHovered.card || !areSetsEqual(currentKeys, globalEventState.lastHovered.keys)) {
            globalEventState.lastHovered = { card, keys: currentKeys };

            if (currentKeys.size > 0) {
                // If we have keys, highlight them.
                animateHighlightState(containerId, card, currentKeys);
            } else {
                // Otherwise, we are on a non-interactive part of the card; reset it.
                animateResetState(containerId, card);
            }
        }
    });

    globalEventState.initialized = true;
}