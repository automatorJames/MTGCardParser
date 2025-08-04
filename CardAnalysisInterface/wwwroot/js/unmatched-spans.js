// unmatched-spans.js

// This is the main orchestration file. It uses the Renderer to draw
// and the Animator to handle events.

const wordTreeObservers = new Map();

function renderTree(containerId, analyzedSpan) {
    const container = document.getElementById(containerId);
    if (!container) { return; }
    container.__data = analyzedSpan;

    if (wordTreeObservers.has(containerId)) {
        recalculateAndDraw(container);
        return;
    }

    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    container.appendChild(svg);
    const resizeObserver = new ResizeObserver(() => recalculateAndDraw(container));
    resizeObserver.observe(container);
    wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });
    recalculateAndDraw(container);
}

function disposeTree(containerId) {
    if (wordTreeObservers.has(containerId)) {
        const { observer, animationFrameId } = wordTreeObservers.get(containerId);
        if (animationFrameId) { cancelAnimationFrame(animationFrameId); }
        observer.disconnect();
        wordTreeObservers.delete(containerId);
    }
}

function recalculateAndDraw(container) {
    const analyzedSpan = container.__data;
    const svg = container.querySelector('svg');
    if (!analyzedSpan || !svg) return;

    svg.innerHTML = '<defs></defs>';
    const config = {
        nodeWidth: 200, nodePadding: 8, mainSpanPadding: 12, nodeHeight: 40, hGap: 40, vGap: 20, cornerRadius: 10,
        mainSpanWidth: 220, mainSpanFontSize: 14, mainSpanLineHeight: 16, mainSpanFill: '#3a3a3a', mainSpanColor: "#e0e0e0", horizontalPadding: 20
    };

    const { keyToColor, allKeys } = prepareColorMap(analyzedSpan);
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0) return;

    const mainSpanObject = { text: analyzedSpan.text, id: 'main-anchor' };

    // Use the renderer module
    const Renderer = window.wordTree.Renderer;
    Renderer.preCalculateAllNodeMetrics(mainSpanObject, true, config, svg);
    analyzedSpan.precedingAdjacencies.forEach(node => Renderer.preCalculateAllNodeMetrics(node, false, config, svg));
    analyzedSpan.followingAdjacencies.forEach(node => Renderer.preCalculateAllNodeMetrics(node, false, config, svg));

    const mainSpanX = 0;
    const precedingResult = Renderer.calculateLayout(analyzedSpan.precedingAdjacencies, 0, mainSpanX, 0, -1, config);
    const followingResult = Renderer.calculateLayout(analyzedSpan.followingAdjacencies, 0, mainSpanX, 0, 1, config);

    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    let minX = mainSpanX - config.mainSpanWidth / 2;
    let maxX = mainSpanX + config.mainSpanWidth / 2;
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
        const newHeight = totalHeight * scaleFactor;
        svg.setAttribute('viewBox', `${minX - config.horizontalPadding} 0 ${naturalContentWidth} ${totalHeight}`);
        container.style.height = `${newHeight}px`;
    }

    const mainSpanY = totalHeight / 2;
    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    Renderer.drawNodesAndConnectors(svg, analyzedSpan.precedingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, -1, config, keyToColor, allKeys, container.id);
    Renderer.drawNodesAndConnectors(svg, analyzedSpan.followingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, 1, config, keyToColor, allKeys, container.id);
    Renderer.createNode(svg, mainSpanObject, mainSpanX, mainSpanY, false, config, keyToColor, container.id);

    setupEventListeners(svg, container.id);
}

function setupEventListeners(svg, containerId) {
    const allKeyedElements = Array.from(svg.querySelectorAll('[data-source-keys]'));

    allKeyedElements.forEach(el => {
        el.__layers = {
            base: el.querySelector('.base-layer') || el,
            highlight: el.querySelector('.highlight-overlay'),
            text: el.querySelector('.node-text')
        };
    });

    const Animator = window.wordTree.Animator;

    svg.addEventListener('mouseover', (e) => {
        const group = e.target.closest('[data-source-keys]');
        if (!group) return;

        const hoveredKeys = new Set(JSON.parse(group.dataset.sourceKeys));
        const elementsToAnimate = new Map();

        allKeyedElements.forEach(el => {
            const elKeys = JSON.parse(el.dataset.sourceKeys);
            const isHighlighted = elKeys.some(key => hoveredKeys.has(key));
            const layers = el.__layers;

            if (isHighlighted) {
                elementsToAnimate.set(layers.base, { start: parseFloat(getComputedStyle(layers.base).opacity), end: 1 });
                if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 1 });
                if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(getComputedStyle(layers.text).opacity), end: 1 });
            } else {
                elementsToAnimate.set(layers.base, { start: parseFloat(getComputedStyle(layers.base).opacity), end: Animator.config.lowlightOpacity });
                if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 0 });
                if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(getComputedStyle(layers.text).opacity), end: Animator.config.lowlightOpacity });
            }
        });
        Animator.animateOpacity(elementsToAnimate, wordTreeObservers.get(containerId));
    });

    svg.addEventListener('mouseout', () => {
        const elementsToAnimate = new Map();
        allKeyedElements.forEach(el => {
            const layers = el.__layers;
            elementsToAnimate.set(layers.base, { start: parseFloat(getComputedStyle(layers.base).opacity), end: 1 });
            if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 0 });
            if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(getComputedStyle(layers.text).opacity), end: 1 });
        });
        Animator.animateOpacity(elementsToAnimate, wordTreeObservers.get(containerId));
    });
}

function prepareColorMap(analyzedSpan) {
    const throughLineColors = ['#9b59b6', '#3498db', '#1abc9c', '#2ecc71', '#f1c40f', '#e67e22', '#8e44ad', '#2980b9', '#16a085', '#27ae60', '#f39c12', '#e74c3c'];
    const colorIndexMap = [0, 2, 4, 6, 8, 10, 1, 3, 5, 7, 9, 11];
    const allKeys = new Set();
    const processNodeForKeys = (node) => {
        if (!node || !node.sourceOccurrenceKeys) return;
        node.sourceOccurrenceKeys.forEach(key => allKeys.add(key));
        if (node.children) node.children.forEach(processNodeForKeys);
    };
    analyzedSpan.precedingAdjacencies.forEach(processNodeForKeys);
    analyzedSpan.followingAdjacencies.forEach(processNodeForKeys);
    const keyToColor = new Map();
    Array.from(allKeys).forEach((key, index) => {
        const colorIdx = colorIndexMap[index % colorIndexMap.length];
        keyToColor.set(key, throughLineColors[colorIdx]);
    });
    return { keyToColor, allKeys };
}