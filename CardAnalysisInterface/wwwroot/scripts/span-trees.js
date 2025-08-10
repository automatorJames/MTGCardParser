// span-trees.ts
import { WordTree } from "./word-tree-animator.js";
import { WordTree as RendererTree } from "./word-tree-renderer.js";
const wordTreeObservers = new Map();
const globalEventSetup = { initialized: false };
// === One-Time Data Processing Function ===
/**
 * Converts the raw AnalyzedSpan from the server into a fully processed,
 * renderer-optimized structure. It creates Maps and Sets and augments the node
 * tree with pre-calculated key sets. This is called only once per card.
 * @param {AnalyzedSpan} rawSpan The raw data from JSON.
 * @returns {ProcessedAnalyzedSpan} The data structure ready for rendering.
 */
function processSpanForClient(rawSpan) {
    // Augment the node tree by adding a pre-calculated 'sourceKeysSet' to each node
    // for efficient use by the renderer.
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
    // Return the final processed object with Maps and Sets.
    return {
        ...rawSpan,
        keyToPaletteMap: new Map(Object.entries(rawSpan.keyToPaletteMap)),
        allKeys: new Set(rawSpan.allKeys),
    };
}
// === Global Event Handlers and Core Logic ===
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
function handleCardNameHover(containerId, card, cardNameItem) {
    const cardName = cardNameItem.dataset.cardName;
    const processedData = card.__data;
    if (!cardName || !processedData)
        return;
    // Use the pre-calculated map from the server data for an instant lookup.
    const keysForCard = processedData.cardNameToKeysMap[cardName] || [];
    animateHighlightState(containerId, card, new Set(keysForCard));
}
function handleCardMouseOut(containerId, card) {
    animateResetState(containerId, card);
}
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
/**
 * Recalculates layout and redraws the SVG. This function is now maximally
 * efficient, using the pre-processed data directly with no on-the-fly conversions.
 */
function recalculateAndDraw(container) {
    const card = container.closest('.span-trees-card');
    const processedData = card?.__data; // This is the fully optimized data object
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
    // EFFICIENT: NO CONVERSIONS.
    // We directly use the pre-calculated Maps and Sets from the processed data object.
    const { keyToPaletteMap, allKeys, text, precedingAdjacencies, followingAdjacencies } = processedData;
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0)
        return;
    const mainSpanObject = { text, id: 'main-anchor' };
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
// === Blazor Interop Functions ===
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
 * Renders all the word trees. It now calls a processing function to convert
 * the raw server data into a client-optimized structure ONCE.
 * @param {AnalyzedSpan[]} spans The collection of raw span data from the server.
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
            // Process the raw data ONCE to create the final, optimized structure.
            card.__data = processSpanForClient(rawSpan);
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
// Expose the Blazor interop functions to the global scope.
window.clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
window.renderAllTrees = renderAllTrees;
//# sourceMappingURL=span-trees.js.map