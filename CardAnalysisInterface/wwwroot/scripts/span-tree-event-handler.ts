// span-tree-event-handler.ts

import { CardElement, WordTreeObserver } from "./models.js";
import { WordTree } from "./word-tree-animator.js";
import { createGradientStops } from "./word-tree-svg-drawer.js";

const wordTreeObservers = new Map<string, WordTreeObserver>();
const globalEventSetup = { initialized: false };

/**
 * Finds the unique container ID for a given card element.
 * @param card The card element to search within.
 * @returns The container ID, or null if not found.
 */
function findContainerIdForCard(card: CardElement): string | null {
    const container = card.querySelector<HTMLElement>('.word-tree-body [id^="word-tree-container-"]');
    return container?.id || null;
}

/**
 * Resets a card's visual state to the default, removing all highlights.
 * @param containerId The ID of the SVG container.
 * @param card The card element to reset.
 */
function animateResetState(containerId: string, card: CardElement): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationController = wordTreeObservers.get(containerId);
    const processedData = card.__data;
    if (!svg || !animationController || !processedData) return;

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
 * Highlights parts of the tree based on a set of filter keys.
 * @param containerId The ID of the SVG container.
 * @param card The parent card element.
 * @param filterKeys The set of source keys to highlight.
 */
function animateHighlightState(containerId: string, card: CardElement, filterKeys: Set<string>): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationController = wordTreeObservers.get(containerId);
    const processedData = card.__data;
    if (!svg || !animationController || !processedData) return;

    const defs = svg.querySelector('defs');
    if (!defs) return;

    const { keyToPaletteMap } = processedData;
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

        const idParts = element.id.split('-');
        const elementType = idParts[1];
        const elementId = idParts[idParts.length - 1];
        const keysForGradient = isHighlighted ? elementKeys.filter((key: string) => filterKeys.has(key)) : [];

        const highlightGradId = `grad-${elementType}-highlight-${containerId}-${elementId}`;
        const highlightGrad = defs.querySelector(`#${highlightGradId}`);
        if (highlightGrad) {
            highlightGrad.innerHTML = createGradientStops(keysForGradient, keyToPaletteMap, 'hexSat', gradientTransitionRatio);
        }
    });

    WordTree.Animator.animateOpacity(elementsToAnimate, animationController);

    card.classList.add('highlight-active');
    const relevantCardNames = new Set<string>();
    filterKeys.forEach(key => {
        relevantCardNames.add(key.substring(0, key.indexOf('[')));
    });

    card.querySelectorAll<HTMLElement>('[data-card-name]').forEach(item => {
        const isRelevant = relevantCardNames.has(item.dataset.cardName || '');
        item.classList.toggle('highlight', isRelevant);
        item.classList.toggle('lowlight', !isRelevant);
    });
}

/**
 * Sets up global event listeners for hover interactions on all word trees.
 * This is initialized only once.
 */
export function setupGlobalEventHandlers(): void {
    if (globalEventSetup.initialized) return;

    document.addEventListener('mouseover', (event: MouseEvent) => {
        if (!(event.target instanceof Element)) return;
        const target: Element = event.target;

        const hoveredSvgGroup = target.closest<HTMLElement>('[data-source-keys]');
        if (hoveredSvgGroup) {
            const card = hoveredSvgGroup.closest<CardElement>('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                const keys = JSON.parse(hoveredSvgGroup.dataset.sourceKeys || '[]') as string[];
                if (containerId) animateHighlightState(containerId, card, new Set(keys));
            }
            return;
        }

        const hoveredCardName = target.closest<HTMLElement>('[data-card-name]');
        if (hoveredCardName) {
            const card = hoveredCardName.closest<CardElement>('.span-trees-card');
            const processedData = card?.__data;
            const cardName = hoveredCardName.dataset.cardName;
            if (card && processedData && cardName) {
                const containerId = findContainerIdForCard(card);
                const keysForCard = processedData.cardNameToKeysMap[cardName] || [];
                if (containerId) animateHighlightState(containerId, card, new Set(keysForCard));
            }
        }
    });

    document.addEventListener('mouseleave', (event: MouseEvent) => {
        if (!(event.target instanceof Element)) return;
        const card = event.target.closest<CardElement>('.span-trees-card');
        if (card) {
            const containerId = findContainerIdForCard(card);
            if (containerId) animateResetState(containerId, card);
        }
    }, true);

    globalEventSetup.initialized = true;
}

// Re-export the observer map for use by the manager.
export { wordTreeObservers };