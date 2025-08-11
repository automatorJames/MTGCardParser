// span-trees.ts 

import { AnalyzedSpan, ProcessedAnalyzedSpan, AdjacencyNode } from "./models.js";
import { WordTree } from "./word-tree-animator.js";
import { WordTree as RendererTree } from "./word-tree-renderer.js";

// === Type Definitions and Module State ===

type CardElement = HTMLElement & { __data?: ProcessedAnalyzedSpan };

interface WordTreeObserver {
    observer: ResizeObserver;
    animationFrameId: number | null;
}
const wordTreeObservers = new Map<string, WordTreeObserver>();
const globalEventSetup = { initialized: false };


// === One-Time Data Processing Function ===

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


// === Global Event Handlers and Core Logic ===

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

function handleCardNameHover(containerId: string, card: CardElement, cardNameItem: HTMLElement): void {
    const cardName = cardNameItem.dataset.cardName;
    const processedData = card.__data;
    if (!cardName || !processedData) return;
    const keysForCard = processedData.cardNameToKeysMap[cardName] || [];
    animateHighlightState(containerId, card, new Set(keysForCard));
}

function handleCardMouseOut(containerId: string, card: CardElement): void {
    animateResetState(containerId, card);
}

function animateHighlightState(containerId: string, card: CardElement, filterKeys: Set<string>): void {
    const svg = document.getElementById(containerId)?.querySelector('svg');
    const animationManager = wordTreeObservers.get(containerId);
    const processedData = card.__data;
    if (!svg || !animationManager || !processedData) return;

    const defs = svg.querySelector('defs');
    if (!defs) return;

    const { keyToPaletteMap } = processedData;
    const config = { gradientTransitionRatio: 0.1 };

    const elementsToAnimate = new Map<HTMLElement, { start: number, end: number }>();
    svg.querySelectorAll<HTMLElement>('[data-source-keys]').forEach(el => {
        const elKeys = JSON.parse(el.dataset.sourceKeys || '[]') as string[];
        const isHighlighted = elKeys.some((k: string) => filterKeys.has(k));

        const baseLayer = el.querySelector<HTMLElement>('.base-layer') || el;
        const highlightOverlay = el.querySelector<HTMLElement>('.highlight-overlay');
        const nodeText = el.querySelector<HTMLElement>('.node-text');
        const targetOpacity = isHighlighted ? 1 : WordTree.Animator.config.lowlightOpacity;
        elementsToAnimate.set(baseLayer, { start: parseFloat(getComputedStyle(baseLayer).opacity), end: targetOpacity });
        if (highlightOverlay) elementsToAnimate.set(highlightOverlay, { start: parseFloat(getComputedStyle(highlightOverlay).opacity), end: isHighlighted ? 1 : 0 });
        if (nodeText) elementsToAnimate.set(nodeText, { start: parseFloat(getComputedStyle(nodeText).opacity), end: targetOpacity });

        const idParts = el.id.split('-');
        const type = idParts[1];
        const elementId = idParts[idParts.length - 1];

        const keysForGradient = isHighlighted ? elKeys.filter((k: string) => filterKeys.has(k)) : elKeys;

        const highlightGradId = `grad-${type}-highlight-${containerId}-${elementId}`;
        const highlightGrad = defs.querySelector(`#${highlightGradId}`);
        if (highlightGrad) {
            highlightGrad.innerHTML = RendererTree.Renderer.createGradientStops(keysForGradient, keyToPaletteMap, 'hexSat', config.gradientTransitionRatio);
        }
    });

    WordTree.Animator.animateOpacity(elementsToAnimate, animationManager);

    card.classList.add('highlight-active');
    const headerNameItems = Array.from(card.querySelectorAll<HTMLElement>('[data-card-name]'));
    const relevantCardNames = new Set<string>();
    filterKeys.forEach(key => {
        relevantCardNames.add(key.substring(0, key.indexOf('[')));
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
    const processedData = card.__data;
    if (!svg || !animationManager || !processedData) return;

    const defs = svg.querySelector('defs');
    if (!defs) return;

    const { keyToPaletteMap } = processedData;
    const config = { gradientTransitionRatio: 0.1 };

    const elementsToAnimate = new Map<HTMLElement, { start: number, end: number }>();
    svg.querySelectorAll<HTMLElement>('.base-layer, .node-text, .highlight-overlay').forEach(el => {
        const isHighlight = el.classList.contains('highlight-overlay');
        elementsToAnimate.set(el as HTMLElement, { start: parseFloat(getComputedStyle(el).opacity), end: isHighlight ? 0 : 1 });
    });
    WordTree.Animator.animateOpacity(elementsToAnimate, animationManager);

    svg.querySelectorAll<HTMLElement>('[data-source-keys]').forEach(el => {
        const elKeys = JSON.parse(el.dataset.sourceKeys || '[]') as string[];
        const idParts = el.id.split('-');
        const type = idParts[1];
        const elementId = idParts[idParts.length - 1];

        const highlightGradId = `grad-${type}-highlight-${containerId}-${elementId}`;
        const highlightGrad = defs.querySelector(`#${highlightGradId}`);
        if (highlightGrad) {
            highlightGrad.innerHTML = RendererTree.Renderer.createGradientStops(elKeys, keyToPaletteMap, 'hexSat', config.gradientTransitionRatio);
        }
    });

    card.classList.remove('highlight-active');
    card.querySelectorAll<HTMLElement>('[data-card-name]').forEach(item => {
        item.classList.remove('highlight', 'lowlight');
    });
}

// helper: build cumulative column push
function buildCumulativePush(raw: Map<number, number>, maxCol: number): Map<number, number> {
    const out = new Map<number, number>();
    let acc = 0;
    for (let c = 1; c <= maxCol; c++) {
        acc += raw.get(c) || 0;
        out.set(c, acc);
    }
    return out;
}

function recalculateAndDraw(container: HTMLElement): void {
    const card = container.closest<CardElement>('.span-trees-card');
    const processedData = card?.__data;
    const svg = container.querySelector('svg');
    if (!processedData || !svg) return;

    svg.innerHTML = '<defs></defs>';
    const config: RendererTree.Renderer.NodeConfig = {
        nodeWidth: 200,
        nodePadding: 8,
        nodeHeight: 40,
        hGap: 40,
        vGap: 20,
        cornerRadius: 10,
        mainSpanFill: '#e0e0e0',
        mainSpanColor: "#e0e0e0",
        horizontalPadding: 20,
        gradientTransitionRatio: 0.1,
        fanGap: 20
    };

    const { keyToPaletteMap, allKeys, text, precedingAdjacencies, followingAdjacencies } = processedData;

    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0) return;

    const mainSpanObject: any = { text, id: 'main-anchor' };
    RendererTree.Renderer.preCalculateAllNodeMetrics(mainSpanObject, true, config, svg);
    precedingAdjacencies.forEach(n => RendererTree.Renderer.preCalculateAllNodeMetrics(n, false, config, svg));
    followingAdjacencies.forEach(n => RendererTree.Renderer.preCalculateAllNodeMetrics(n, false, config, svg));

    const precedingResult = RendererTree.Renderer.calculateLayout(precedingAdjacencies, 0, 0, 0, -1, config);
    const followingResult = RendererTree.Renderer.calculateLayout(followingAdjacencies, 0, 0, 0, 1, config);

    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    const mainSpanY = totalHeight / 2;

    // Apply Y offset first (needed for fan direction classification)
    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    // Compute RAW per-column pushes (per side) and assign fan deltas
    const followingRawPush = RendererTree.Renderer.computeColumnOffsetsAndAssignFan(followingAdjacencies, 0, mainSpanY, 1, config);
    const precedingRawPush = RendererTree.Renderer.computeColumnOffsetsAndAssignFan(precedingAdjacencies, 0, mainSpanY, -1, config);

    // Build cumulative pushes so each farther column includes all prior columns' push
    const maxColFollowing = followingResult.layout.reduce((m, n) => Math.max(m, RendererTree.Renderer.getColumnIndex(n)), 0);
    const maxColPreceding = precedingResult.layout.reduce((m, n) => Math.max(m, RendererTree.Renderer.getColumnIndex(n)), 0);

    const followingPushCum = buildCumulativePush(followingRawPush, maxColFollowing);
    const precedingPushCum = buildCumulativePush(precedingRawPush, maxColPreceding);

    // Shift columns outward while preserving vertical alignment within each column
    precedingResult.layout.forEach(n => {
        const col = RendererTree.Renderer.getColumnIndex(n);
        const push = precedingPushCum.get(col) || 0;
        n.layout.x += -push; // outward to the left
    });
    followingResult.layout.forEach(n => {
        const col = RendererTree.Renderer.getColumnIndex(n);
        const push = followingPushCum.get(col) || 0;
        n.layout.x += push; // outward to the right
    });

    // Compute extents AFTER applying column pushes
    let minX = -config.nodeWidth / 2;
    let maxX = config.nodeWidth / 2;
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

    RendererTree.Renderer.drawNodesAndConnectors(svg, precedingAdjacencies, mainSpanObject, 0, mainSpanY, -1, config, keyToPaletteMap, allKeys, container.id);
    RendererTree.Renderer.drawNodesAndConnectors(svg, followingAdjacencies, mainSpanObject, 0, mainSpanY, 1, config, keyToPaletteMap, allKeys, container.id);
    RendererTree.Renderer.createNode(svg, mainSpanObject, 0, mainSpanY, false, config, keyToPaletteMap, container.id);
}


// === Blazor Interop Functions ===

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

export function renderAllTrees(spans: AnalyzedSpan[]): void {
    spans.forEach((rawSpan, index) => {
        const containerId = `word-tree-container-${index}`;
        const spinnerId = `spinner-${index}`;
        const container = document.getElementById(containerId);
        const spinner = document.getElementById(spinnerId);

        if (!container) return;

        const card = container.closest<CardElement>('.span-trees-card');
        if (card) {
            card.__data = processSpanForClient(rawSpan);
        }

        if (!wordTreeObservers.has(containerId)) {
            const resizeObserver = new ResizeObserver(() => recalculateAndDraw(container));
            resizeObserver.observe(container);
            wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });
        }

        const svgNS = "http://www.w3.org/2000/svg";
        const svg = document.createElementNS(svgNS, 'svg');
        container.appendChild(svg);
        recalculateAndDraw(container);

        if (spinner) {
            spinner.style.display = 'none';
        }
    });
}

// Expose the Blazor interop functions to the global scope.
(window as any).clearAllTreesAndShowSpinners = clearAllTreesAndShowSpinners;
(window as any).renderAllTrees = renderAllTrees;
