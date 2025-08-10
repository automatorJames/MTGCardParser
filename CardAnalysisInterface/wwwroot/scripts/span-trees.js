import { WordTree } from "./word-tree-animator.js";
import { WordTree as RendererTree } from "./word-tree-renderer.js";
const wordTreeObservers = new Map();
const globalEventSetup = { initialized: false };
// === NEW: One-Time Data Pre-processing Function ===
/**
 * Traverses the node tree once to create efficient lookup maps.
 * This avoids expensive re-computation on every render or interaction.
 * @param {AnalyzedSpan} span The raw data for the span.
 * @returns {ProcessedAnalyzedSpan} The processed data with lookup maps.
 */
function preprocessSpanData(span) {
    const keyToPaletteMap = new Map();
    const allKeys = new Set();
    const cardNameToKeysMap = new Map();
    const traverse = (node) => {
        if (!node)
            return;
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
            cardNameToKeysMap.get(cardName).add(key);
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
function setupGlobalEventHandlers() {
    if (globalEventSetup.initialized)
        return;
    document.addEventListener('mouseover', (e) => {
        if (!(e.target instanceof Element))
            return;
        const target = e.target;
        const svgGroup = target.closest('[data-source-keys]');
        if (svgGroup) {
            const card = svgGroup.closest('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                if (containerId)
                    handleNodeHover(containerId, card, svgGroup);
            }
            return;
        }
        const cardNameItem = target.closest('[data-card-name]');
        if (cardNameItem) {
            const card = cardNameItem.closest('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                if (containerId)
                    handleCardNameHover(containerId, card, cardNameItem);
            }
        }
    });
    document.addEventListener('mouseleave', (e) => {
        if (!(e.target instanceof Element))
            return;
        const target = e.target;
        const card = target.closest('.span-trees-card');
        if (card) {
            const containerId = findContainerIdForCard(card);
            if (containerId)
                handleCardMouseOut(containerId, card);
        }
    }, true);
    globalEventSetup.initialized = true;
}
function findContainerIdForCard(card) {
    const container = card.querySelector('.word-tree-body [id^="word-tree-container-"]');
    return container?.id || null;
}
function handleNodeHover(containerId, card, svgGroup) {
    const keys = JSON.parse(svgGroup.dataset.sourceKeys || '[]');
    animateHighlightState(containerId, card, new Set(keys));
}
/** OPTIMIZED: Uses a pre-computed map for an efficient O(1) lookup. */
function handleCardNameHover(containerId, card, cardNameItem) {
    const cardName = cardNameItem.dataset.cardName;
    const processedData = card.__data;
    if (!cardName || !processedData)
        return;
    // EFFICIENT: Get all keys for the card from the pre-computed map. No tree traversal needed.
    const keys = processedData.cardNameToKeysMap.get(cardName) || new Set();
    animateHighlightState(containerId, card, keys);
}
function handleCardMouseOut(containerId, card) {
    animateResetState(containerId, card);
}
/** OPTIMIZED: Derives highlighted card names efficiently from activeKeys. */
function animateHighlightState(containerId, card, activeKeys) {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationManager = wordTreeObservers.get(containerId);
    if (!svg || !animationManager)
        return;
    const elementsToAnimate = new Map();
    svg.querySelectorAll('[data-source-keys]').forEach(el => {
        const elKeys = JSON.parse(el.dataset.sourceKeys || '[]');
        const isHighlighted = elKeys.some((k) => activeKeys.has(k));
        const baseLayer = el.querySelector('.base-layer') || el;
        const highlightOverlay = el.querySelector('.highlight-overlay');
        const nodeText = el.querySelector('.node-text');
        const targetOpacity = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;
        elementsToAnimate.set(baseLayer, { start: parseFloat(getComputedStyle(baseLayer).opacity), end: targetOpacity });
        if (highlightOverlay)
            elementsToAnimate.set(highlightOverlay, { start: parseFloat(getComputedStyle(highlightOverlay).opacity), end: isHighlighted ? 1 : 0 });
        if (nodeText)
            elementsToAnimate.set(nodeText, { start: parseFloat(getComputedStyle(nodeText).opacity), end: targetOpacity });
    });
    WordTree.Animator.animateOpacity(elementsToAnimate, animationManager);
    card.classList.add('highlight-active');
    const headerNameItems = Array.from(card.querySelectorAll('[data-card-name]'));
    // EFFICIENT: Determine relevant card names from the small activeKeys set. No tree traversal needed.
    const relevantCardNames = new Set();
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
function animateResetState(containerId, card) {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationManager = wordTreeObservers.get(containerId);
    if (!svg || !animationManager)
        return;
    const elementsToAnimate = new Map();
    svg.querySelectorAll('.base-layer, .node-text, .highlight-overlay').forEach(el => {
        const isHighlight = el.classList.contains('highlight-overlay');
        elementsToAnimate.set(el, { start: parseFloat(getComputedStyle(el).opacity), end: isHighlight ? 0 : 1 });
    });
    WordTree.Animator.animateOpacity(elementsToAnimate, animationManager);
    card.classList.remove('highlight-active');
    card.querySelectorAll('[data-card-name]').forEach(item => {
        item.classList.remove('highlight', 'lowlight');
    });
}
/** OPTIMIZED: Uses pre-calculated data maps for drawing. */
function recalculateAndDraw(container) {
    const card = container.closest('.span-trees-card');
    const processedData = card?.__data;
    const svg = container.querySelector('svg');
    if (!processedData || !svg)
        return;
    svg.innerHTML = '<defs></defs>';
    const config = {
        nodeWidth: 200, nodePadding: 8, mainSpanPadding: 12,
        nodeHeight: 40, hGap: 40, vGap: 20, cornerRadius: 10,
        mainSpanWidth: 220, mainSpanFontSize: 14, mainSpanLineHeight: 16,
        mainSpanFill: '#3a3a3a', mainSpanColor: "#e0e0e0",
        horizontalPadding: 20, gradientTransitionRatio: 0.1
    };
    // EFFICIENT: Use the pre-calculated maps directly. No more 'gatherKeys' traversal.
    const { keyToPaletteMap, allKeys, text, precedingAdjacencies, followingAdjacencies } = processedData;
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0)
        return;
    const mainSpanObject = { text: text, id: 'main-anchor' };
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
    }
    else {
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
export function clearAllTreesAndShowSpinners(count) {
    setupGlobalEventHandlers();
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
export function renderAllTrees(spans) {
    spans.forEach((analyzedSpan, index) => {
        const containerId = `word-tree-container-${index}`;
        const spinnerId = `spinner-${index}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);
        if (!container)
            return;
        const card = container.closest('.span-trees-card');
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
window.clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
window.renderAllTrees = renderAllTrees;
//# sourceMappingURL=span-trees.js.map