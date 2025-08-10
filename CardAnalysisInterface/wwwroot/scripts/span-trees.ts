import { AnalyzedSpan, AdjacencyNode, DeterministicPalette } from "./models.js";
import { WordTree } from "./word-tree-animator.js";
import { WordTree as RendererTree } from "./word-tree-renderer.js";

// === Type Definitions and Module State ===

/**
 * Extends the base AnalyzedSpan with pre-calculated lookup maps for efficient
 * rendering and event handling. This processing is done once per card.
 */
interface ProcessedAnalyzedSpan extends AnalyzedSpan {
    keyToPaletteMap: Map<string, DeterministicPalette>;
    allKeys: Set<string>;
    cardNameToKeysMap: Map<string, Set<string>>;
}

type CardElement = HTMLElement & { __data?: ProcessedAnalyzedSpan };

interface WordTreeObserver {
    observer: ResizeObserver;
    animationFrameId: number | null;
}
const wordTreeObservers = new Map<string, WordTreeObserver>();
const globalEventSetup = { initialized: false };


// === NEW: One-Time Data Pre-processing Function ===

/**
 * Traverses the node tree once to create efficient lookup maps.
 * This avoids expensive re-computation on every render or interaction.
 * @param {AnalyzedSpan} span The raw data for the span.
 * @returns {ProcessedAnalyzedSpan} The processed data with lookup maps.
 */
function preprocessSpanData(span: AnalyzedSpan): ProcessedAnalyzedSpan {
    const keyToPaletteMap = new Map<string, DeterministicPalette>();
    const allKeys = new Set<string>();
    const cardNameToKeysMap = new Map<string, Set<string>>();

    const traverse = (node: AdjacencyNode) => {
        if (!node) return;

        node.sourceOccurrenceKeys.forEach(key => {
            allKeys.add(key);
            const cardName = key.substring(0, key.indexOf('['));

            // Map the full key to its palette
            const palette = span.cardPalettes[cardName];
            if (palette) {
                keyToPaletteMap.set(key, palette);
            }

            // Map the card name to its set of keys
            if (!cardNameToKeysMap.has(cardName)) {
                cardNameToKeysMap.set(cardName, new Set());
            }
            cardNameToKeysMap.get(cardName)!.add(key);
        });

        node.children?.forEach(traverse);
    };

    span.precedingAdjacencies.forEach(traverse);
    span.followingAdjacencies.forEach(traverse);

    return {
        ...span,
        keyToPaletteMap,
        allKeys,
        cardNameToKeysMap,
    };
}


// === Global Event Handlers, Animation, and Drawing Logic (Optimized) ===

function setupGlobalEventHandlers(): void {
    if (globalEventSetup.initialized) return;

    document.addEventListener('mouseover', (e: MouseEvent) => {
        if (!(e.target instanceof Element)) return;
        const target: Element = e.target;

        const svgGroup = target.closest<HTMLElement>('[data-source-keys]');
        if (svgGroup) {
            const card = svgGroup.closest<CardElement>('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                if (containerId) handleNodeHover(containerId, card, svgGroup);
            }
            return;
        }

        const cardNameItem = target.closest<HTMLElement>('[data-card-name]');
        if (cardNameItem) {
            const card = cardNameItem.closest<CardElement>('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                if (containerId) handleCardNameHover(containerId, card, cardNameItem);
            }
        }
    });

    document.addEventListener('mouseleave', (e: MouseEvent) => {
        if (!(e.target instanceof Element)) return;
        const target: Element = e.target;
        const card = target.closest<CardElement>('.span-trees-card');
        if (card) {
            const containerId = findContainerIdForCard(card);
            if (containerId) handleCardMouseOut(containerId, card);
        }
    }, true);

    globalEventSetup.initialized = true;
}

function findContainerIdForCard(card: CardElement): string | null {
    const container = card.querySelector<HTMLElement>('.word-tree-body [id^="word-tree-container-"]');
    return container?.id || null;
}

function handleNodeHover(containerId: string, card: CardElement, svgGroup: HTMLElement): void {
    const keys = JSON.parse(svgGroup.dataset.sourceKeys || '[]') as string[];
    animateHighlightState(containerId, card, new Set(keys));
}

/** OPTIMIZED: Uses a pre-computed map for an efficient O(1) lookup. */
function handleCardNameHover(containerId: string, card: CardElement, cardNameItem: HTMLElement): void {
    const cardName = cardNameItem.dataset.cardName;
    const processedData = card.__data;
    if (!cardName || !processedData) return;

    // EFFICIENT: Get all keys for the card from the pre-computed map. No tree traversal needed.
    const keys = processedData.cardNameToKeysMap.get(cardName) || new Set<string>();

    animateHighlightState(containerId, card, keys);
}

function handleCardMouseOut(containerId: string, card: CardElement): void {
    animateResetState(containerId, card);
}

/** OPTIMIZED: Derives highlighted card names efficiently from activeKeys. */
function animateHighlightState(containerId: string, card: CardElement, activeKeys: Set<string>): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationManager = wordTreeObservers.get(containerId);
    if (!svg || !animationManager) return;

    const elementsToAnimate = new Map<HTMLElement, { start: number, end: number }>();
    svg.querySelectorAll<HTMLElement>('[data-source-keys]').forEach(el => {
        const elKeys = JSON.parse(el.dataset.sourceKeys || '[]') as string[];
        const isHighlighted = elKeys.some((k: string) => activeKeys.has(k));
        const baseLayer = el.querySelector<HTMLElement>('.base-layer') || el;
        const highlightOverlay = el.querySelector<HTMLElement>('.highlight-overlay');
        const nodeText = el.querySelector<HTMLElement>('.node-text');

        const targetOpacity = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;

        elementsToAnimate.set(baseLayer, { start: parseFloat(getComputedStyle(baseLayer).opacity), end: targetOpacity });
        if (highlightOverlay) elementsToAnimate.set(highlightOverlay, { start: parseFloat(getComputedStyle(highlightOverlay).opacity), end: isHighlighted ? 1 : 0 });
        if (nodeText) elementsToAnimate.set(nodeText, { start: parseFloat(getComputedStyle(nodeText).opacity), end: targetOpacity });
    });

    WordTree.Animator.animateOpacity(elementsToAnimate, animationManager);

    card.classList.add('highlight-active');
    const headerNameItems = Array.from(card.querySelectorAll<HTMLElement>('[data-card-name]'));

    // EFFICIENT: Determine relevant card names from the small activeKeys set. No tree traversal needed.
    const relevantCardNames = new Set<string>();
    activeKeys.forEach(key => {
        const cardName = key.substring(0, key.indexOf('['));
        relevantCardNames.add(cardName);
    });

    headerNameItems.forEach(item => {
        const isRelevant = relevantCardNames.has(item.dataset.cardName || '');
        item.classList.toggle('highlight', isRelevant);
        item.classList.toggle('lowlight', !isRelevant);
    });
}

function animateResetState(containerId: string, card: CardElement): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationManager = wordTreeObservers.get(containerId);
    if (!svg || !animationManager) return;

    const elementsToAnimate = new Map<HTMLElement, { start: number, end: number }>();
    svg.querySelectorAll<HTMLElement>('.base-layer, .node-text, .highlight-overlay').forEach(el => {
        const isHighlight = el.classList.contains('highlight-overlay');
        elementsToAnimate.set(el as HTMLElement, { start: parseFloat(getComputedStyle(el).opacity), end: isHighlight ? 0 : 1 });
    });

    WordTree.Animator.animateOpacity(elementsToAnimate, animationManager);

    card.classList.remove('highlight-active');
    card.querySelectorAll<HTMLElement>('[data-card-name]').forEach(item => {
        item.classList.remove('highlight', 'lowlight');
    });
}

/** OPTIMIZED: Uses pre-calculated data maps for drawing. */
function recalculateAndDraw(container: HTMLElement): void {
    const card = container.closest<CardElement>('.span-trees-card');
    const processedData = card?.__data;
    const svg = container.querySelector('svg');
    if (!processedData || !svg) return;

    svg.innerHTML = '<defs></defs>';
    const config: RendererTree.Renderer.NodeConfig = {
        nodeWidth: 200, nodePadding: 8, mainSpanPadding: 12,
        nodeHeight: 40, hGap: 40, vGap: 20, cornerRadius: 10,
        mainSpanWidth: 220, mainSpanFontSize: 14, mainSpanLineHeight: 16,
        mainSpanFill: '#3a3a3a', mainSpanColor: "#e0e0e0",
        horizontalPadding: 20, gradientTransitionRatio: 0.1
    };

    // EFFICIENT: Use the pre-calculated maps directly. No more 'gatherKeys' traversal.
    const { keyToPaletteMap, allKeys, text, precedingAdjacencies, followingAdjacencies } = processedData;

    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0) return;

    const mainSpanObject: any = { text: text, id: 'main-anchor' };
    RendererTree.Renderer.preCalculateAllNodeMetrics(mainSpanObject, true, config, svg);
    precedingAdjacencies.forEach(n => RendererTree.Renderer.preCalculateAllNodeMetrics(n, false, config, svg));
    followingAdjacencies.forEach(n => RendererTree.Renderer.preCalculateAllNodeMetrics(n, false, config, svg));

    const precedingResult = RendererTree.Renderer.calculateLayout(precedingAdjacencies, 0, 0, 0, -1, config);
    const followingResult = RendererTree.Renderer.calculateLayout(followingAdjacencies, 0, 0, 0, 1, config);

    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    const mainSpanY = totalHeight / 2;

    let minX = -config.mainSpanWidth / 2;
    let maxX = config.mainSpanWidth / 2;
    [...precedingResult.layout, ...followingResult.layout].forEach(node => {
        minX = Math.min(minX, node.layout.x - config.nodeWidth / 2);
        maxX = Math.max(maxX, node.layout.x + config.nodeWidth / 2);
    });

    const naturalTreeWidth = maxX - minX;
    const naturalContentWidth = naturalTreeWidth + config.horizontalPadding * 2;
    if (naturalContentWidth <= availableWidth) {
        const margin = (availableWidth - naturalTreeWidth) / 2;
        svg.setAttribute('viewBox', `${minX - margin} 0 ${availableWidth} ${totalHeight}`);
        container.style.height = `${totalHeight}px`;
    } else {
        const scaleFactor = availableWidth / naturalContentWidth;
        svg.setAttribute('viewBox', `${minX - config.horizontalPadding} 0 ${naturalContentWidth} ${totalHeight}`);
        container.style.height = `${totalHeight * scaleFactor}px`;
    }

    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    RendererTree.Renderer.drawNodesAndConnectors(svg, precedingAdjacencies, mainSpanObject, 0, mainSpanY, -1, config, keyToPaletteMap, allKeys, container.id);
    RendererTree.Renderer.drawNodesAndConnectors(svg, followingAdjacencies, mainSpanObject, 0, mainSpanY, 1, config, keyToPaletteMap, allKeys, container.id);
    RendererTree.Renderer.createNode(svg, mainSpanObject, 0, mainSpanY, false, config, keyToPaletteMap, container.id);
}


// === Blazor Interop Functions (Optimized) ===

export function clearAllTreesAndShowSpinners(count: number): void {
    setupGlobalEventHandlers();
    for (const id of wordTreeObservers.keys()) {
        const index = parseInt(id.split('-').pop() || '-1');
        if (index >= count) {
            const observerData = wordTreeObservers.get(id);
            if (observerData) {
                observerData.observer.disconnect();
                if (observerData.animationFrameId) cancelAnimationFrame(observerData.animationFrameId);
                wordTreeObservers.delete(id);
            }
        }
    }
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
 * Renders all the word trees. This function now pre-processes the data
 * for each tree to enable efficient rendering and interactions.
 * @param {AnalyzedSpan[]} spans The collection of span data to render.
 */
export function renderAllTrees(spans: AnalyzedSpan[]): void {
    spans.forEach((analyzedSpan, index) => {
        const containerId = `word-tree-container-${index}`;
        const spinnerId = `spinner-${index}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);

        if (!container) return;

        const card = container.closest<CardElement>('.span-trees-card');
        if (card) {
            // Pre-process the raw data into our optimized structure.
            card.__data = preprocessSpanData(analyzedSpan);
        }

        if (!wordTreeObservers.has(containerId)) {
            const resizeObserver = new ResizeObserver(() => recalculateAndDraw(container));
            resizeObserver.observe(container);
            wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });
        }

        const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
        container.appendChild(svg);
        recalculateAndDraw(container);

        if (spinner) {
            spinner.style.display = 'none';
        }
    });
}

(window as any).clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
(window as any).renderAllTrees = renderAllTrees;