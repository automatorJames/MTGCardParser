// This is the main orchestration file. It uses the Renderer to draw
// and the Animator to handle events.

const wordTreeObservers = new Map();

function renderTree(containerId, analyzedSpan) {
    const container = document.getElementById(containerId);
    if (!container) { return; }

    // Store the data on the overall card element for easy access
    const card = container.closest('.unmatched-spans-card');
    if (card) {
        card.__data = analyzedSpan;
    }

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
    const container = document.getElementById(containerId);
    if (container) {
        const card = container.closest('.unmatched-spans-card');
        if (card) card.__data = null;
    }
}

function recalculateAndDraw(container) {
    const card = container.closest('.unmatched-spans-card');
    const analyzedSpan = card ? card.__data : null;
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
    const Animator = window.wordTree.Animator;
    const animationManager = wordTreeObservers.get(containerId);
    const card = document.getElementById(containerId).closest('.unmatched-spans-card');
    const headerNameItems = Array.from(card.querySelectorAll('[data-card-name]'));
    const allKeyedSVGElements = Array.from(svg.querySelectorAll('[data-source-keys]'));

    allKeyedSVGElements.forEach(el => {
        el.__layers = {
            base: el.querySelector('.base-layer') || el,
            highlight: el.querySelector('.highlight-overlay'),
            text: el.querySelector('.node-text')
        };
    });

    const animateState = (activeKeys) => {
        // Animate SVG Nodes
        const elementsToAnimate = new Map();
        allKeyedSVGElements.forEach(el => {
            const elKeys = JSON.parse(el.dataset.sourceKeys);
            const isHighlighted = elKeys.some(key => activeKeys.has(key));
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
        Animator.animateOpacity(elementsToAnimate, animationManager);

        // Highlight Header
        card.classList.add('highlight-active');
        const relevantCardNames = new Set();
        activeKeys.forEach(k => relevantCardNames.add(k.substring(0, k.indexOf('['))));
        headerNameItems.forEach(item => {
            const isRelevant = relevantCardNames.has(item.dataset.cardName);
            item.classList.toggle('highlight', isRelevant);
            item.classList.toggle('lowlight', !isRelevant);
        });
    };

    const resetState = () => {
        // Reset SVG Nodes
        const elementsToAnimate = new Map();
        allKeyedSVGElements.forEach(el => {
            const layers = el.__layers;
            elementsToAnimate.set(layers.base, { start: parseFloat(getComputedStyle(layers.base).opacity), end: 1 });
            if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 0 });
            if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(getComputedStyle(layers.text).opacity), end: 1 });
        });
        Animator.animateOpacity(elementsToAnimate, animationManager);

        // Reset Header
        card.classList.remove('highlight-active');
        headerNameItems.forEach(item => item.classList.remove('highlight', 'lowlight'));
    };

    // --- Event Listener Attachments ---

    svg.addEventListener('mouseover', e => {
        const group = e.target.closest('[data-source-keys]');
        if (group) {
            animateState(new Set(JSON.parse(group.dataset.sourceKeys)));
        }
    });

    headerNameItems.forEach(item => {
        item.addEventListener('mouseover', () => {
            const cardName = item.dataset.cardName;
            const keysForCard = new Set();
            allKeyedSVGElements.forEach(el => {
                const elKeys = JSON.parse(el.dataset.sourceKeys);
                elKeys.forEach(key => {
                    if (key.startsWith(cardName + '[')) {
                        keysForCard.add(key);
                    }
                });
            });
            animateState(keysForCard);
        });
    });

    card.addEventListener('mouseout', resetState);
}

function prepareColorMap(analyzedSpan) {
    const allKeys = new Set();
    const processNodeForKeys = (node) => {
        if (!node || !node.sourceOccurrenceKeys) return;
        node.sourceOccurrenceKeys.forEach(key => allKeys.add(key));
        if (node.children) node.children.forEach(processNodeForKeys);
    };
    analyzedSpan.precedingAdjacencies.forEach(processNodeForKeys);
    analyzedSpan.followingAdjacencies.forEach(processNodeForKeys);

    const keyToColor = new Map();
    const cardColors = analyzedSpan.cardColors || {}; // Use the new property from the server

    Array.from(allKeys).forEach(key => {
        // Extract card name from a key like "CardName[1..5]"
        const cardName = key.substring(0, key.indexOf('['));
        const color = cardColors[cardName] || '#dddddd'; // Default color
        keyToColor.set(key, color);
    });

    return { keyToColor, allKeys };
}