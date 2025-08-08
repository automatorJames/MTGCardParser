// span-trees.js

// === Observer storage ===
const wordTreeObservers = new Map();
const globalEventSetup = { initialized: false };

// === Global event delegation setup ===
function setupGlobalEventHandlers() {
    if (globalEventSetup.initialized) return;

    document.addEventListener('mouseover', function (e) {
        // Handle SVG node hover
        const svgGroup = e.target.closest('[data-source-keys]');
        if (svgGroup) {
            // Find card by traversing up from the SVG group
            const card = svgGroup.closest('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                if (containerId) {
                    handleNodeHover(containerId, card, svgGroup);
                }
            }
            return;
        }

        // Handle header card name hover
        const cardNameItem = e.target.closest('[data-card-name]');
        if (cardNameItem) {
            // Find card by traversing up from the header item
            const card = cardNameItem.closest('.span-trees-card');
            if (card) {
                const containerId = findContainerIdForCard(card);
                if (containerId) {
                    handleCardNameHover(containerId, card, cardNameItem);
                }
            }
            return;
        }
    });

    document.addEventListener('mouseleave', function (e) {
        // Ensure we have an element, not a text node or other non-element
        const target = e.target && e.target.nodeType === Node.ELEMENT_NODE ? e.target : null;
        if (!target) return;

        // Check if leaving a card
        const card = target.closest('.span-trees-card');
        if (card) {
            const containerId = findContainerIdForCard(card);
            if (containerId) {
                handleCardMouseOut(containerId, card);
            }
            return;
        }

        // Check if leaving an SVG node
        const svgGroup = target.closest('[data-source-keys]');
        if (svgGroup) {
            const cardFromSvg = svgGroup.closest('.span-trees-card');
            if (cardFromSvg) {
                const containerId = findContainerIdForCard(cardFromSvg);
                if (containerId) {
                    handleCardMouseOut(containerId, cardFromSvg);
                }
            }
            return;
        }

        // Check if leaving a card name item
        const cardNameItem = target.closest('[data-card-name]');
        if (cardNameItem) {
            const cardFromHeader = cardNameItem.closest('.span-trees-card');
            if (cardFromHeader) {
                const containerId = findContainerIdForCard(cardFromHeader);
                if (containerId) {
                    handleCardMouseOut(containerId, cardFromHeader);
                }
            }
            return;
        }
    }, true); // Use capture phase to ensure we catch the event

    globalEventSetup.initialized = true;
}

// === Helper function to find container ID for a card ===
function findContainerIdForCard(card) {
    // Try to find the container element within this card
    const container = card.querySelector('.word-tree-body [id^="word-tree-container-"]');
    if (container && container.id) {
        return container.id;
    }

    // If that doesn't work, look for any container that has this card as an ancestor
    for (const [containerId, observerData] of wordTreeObservers.entries()) {
        const containerEl = document.getElementById(containerId);
        if (containerEl && containerEl.closest('.span-trees-card') === card) {
            return containerId;
        }
    }

    return null;
}

// === Event handlers ===
function handleNodeHover(containerId, card, svgGroup) {
    const keys = JSON.parse(svgGroup.dataset.sourceKeys);
    animateHighlightState(containerId, card, new Set(keys));
}

function handleCardNameHover(containerId, card, cardNameItem) {
    const cardName = cardNameItem.dataset.cardName;

    const container = document.getElementById(containerId);
    const svg = container?.querySelector('svg');
    if (!svg) return;

    const keys = new Set();
    const allKeyedSVGElements = Array.from(svg.querySelectorAll('[data-source-keys]'));
    allKeyedSVGElements.forEach(el => {
        JSON.parse(el.dataset.sourceKeys).forEach(k => {
            if (k.startsWith(cardName + '[')) keys.add(k);
        });
    });

    animateHighlightState(containerId, card, keys);
}

function handleCardMouseOut(containerId, card) {
    animateResetState(containerId, card);
}

// === Animation functions ===
function animateHighlightState(containerId, card, activeKeys) {
    const container = document.getElementById(containerId);
    const svg = container?.querySelector('svg');
    if (!svg || !card) return;

    const Animator = window.wordTree.Animator;
    const animationManager = wordTreeObservers.get(containerId);

    const allKeyedSVGElements = Array.from(svg.querySelectorAll('[data-source-keys]'));
    const headerNameItems = Array.from(card.querySelectorAll('[data-card-name]'));

    // Prepare layers if not already done
    allKeyedSVGElements.forEach(el => {
        if (!el.__layers) {
            el.__layers = {
                base: el.querySelector('.base-layer') || el,
                highlight: el.querySelector('.highlight-overlay'),
                text: el.querySelector('.node-text')
            };
        }
    });

    const elementsToAnimate = new Map();
    allKeyedSVGElements.forEach(el => {
        const elKeys = JSON.parse(el.dataset.sourceKeys);
        const isHighlighted = elKeys.some(k => activeKeys.has(k));
        const layers = el.__layers;

        if (isHighlighted) {
            elementsToAnimate.set(
                layers.base,
                { start: parseFloat(getComputedStyle(layers.base).opacity), end: 1 });
            if (layers.highlight) elementsToAnimate.set(
                layers.highlight,
                { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 1 });
            if (layers.text) elementsToAnimate.set(
                layers.text,
                { start: parseFloat(getComputedStyle(layers.text).opacity), end: 1 });
        } else {
            elementsToAnimate.set(
                layers.base,
                { start: parseFloat(getComputedStyle(layers.base).opacity), end: Animator.config.lowlightOpacity });
            if (layers.highlight) elementsToAnimate.set(
                layers.highlight,
                { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 0 });
            if (layers.text) elementsToAnimate.set(
                layers.text,
                { start: parseFloat(getComputedStyle(layers.text).opacity), end: Animator.config.lowlightOpacity });
        }
    });

    if (animationManager) {
        Animator.animateOpacity(elementsToAnimate, animationManager);
    }

    card.classList.add('highlight-active');

    const relevant = new Set();
    activeKeys.forEach(k => relevant.add(k.substring(0, k.indexOf('['))));
    headerNameItems.forEach(item => {
        const isRel = relevant.has(item.dataset.cardName);
        item.classList.toggle('highlight', isRel);
        item.classList.toggle('lowlight', !isRel);
    });
}

function animateResetState(containerId, card) {
    const container = document.getElementById(containerId);
    const svg = container?.querySelector('svg');
    if (!svg || !card) return;

    const Animator = window.wordTree.Animator;
    const animationManager = wordTreeObservers.get(containerId);

    const allKeyedSVGElements = Array.from(svg.querySelectorAll('[data-source-keys]'));
    const headerNameItems = Array.from(card.querySelectorAll('[data-card-name]'));

    const elementsToAnimate = new Map();
    allKeyedSVGElements.forEach(el => {
        if (!el.__layers) return;
        const layers = el.__layers;
        elementsToAnimate.set(
            layers.base,
            { start: parseFloat(getComputedStyle(layers.base).opacity), end: 1 });
        if (layers.highlight) elementsToAnimate.set(
            layers.highlight,
            { start: parseFloat(getComputedStyle(layers.highlight).opacity), end: 0 });
        if (layers.text) elementsToAnimate.set(
            layers.text,
            { start: parseFloat(getComputedStyle(layers.text).opacity), end: 1 });
    });

    // Force immediate reset if no animation manager, or if animation fails
    if (!animationManager) {
        elementsToAnimate.forEach((targets, element) => {
            if (element) {
                element.style.opacity = targets.end;
            }
        });
    } else {
        Animator.animateOpacity(elementsToAnimate, animationManager);
    }

    card.classList.remove('highlight-active');
    headerNameItems.forEach(item => item.classList.remove('highlight', 'lowlight'));
}

// === Main render entrypoint ===
function renderTree(containerId, analyzedSpan) {
    setupGlobalEventHandlers();

    const container = document.getElementById(containerId);
    if (!container) return;

    // Attach data to card
    const card = container.closest('.span-trees-card');
    if (card) card.__data = analyzedSpan;

    // If already observing, just redraw
    if (wordTreeObservers.has(containerId)) {
        recalculateAndDraw(container);
        return;
    }

    // First‐time setup
    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    container.appendChild(svg);

    const resizeObserver = new ResizeObserver(() => recalculateAndDraw(container));
    resizeObserver.observe(container);
    wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });

    recalculateAndDraw(container);
}

// === Disposal ===
function disposeTree(containerId) {
    if (wordTreeObservers.has(containerId)) {
        const { observer, animationFrameId } = wordTreeObservers.get(containerId);
        if (animationFrameId) cancelAnimationFrame(animationFrameId);
        observer.disconnect();
        wordTreeObservers.delete(containerId);
    }
    const container = document.getElementById(containerId);
    if (container) {
        const card = container.closest('.span-trees-card');
        if (card) card.__data = null;
    }
}

// === Layout & Draw ===
function recalculateAndDraw(container) {
    const card = container.closest('.span-trees-card');
    const analyzedSpan = card ? card.__data : null;
    const svg = container.querySelector('svg');
    if (!analyzedSpan || !svg) return;

    svg.innerHTML = '<defs></defs>';
    const config = {
        nodeWidth: 200, nodePadding: 8, mainSpanPadding: 12,
        nodeHeight: 40, hGap: 40, vGap: 20, cornerRadius: 10,
        mainSpanWidth: 220, mainSpanFontSize: 14, mainSpanLineHeight: 16,
        mainSpanFill: '#3a3a3a', mainSpanColor: "#e0e0e0",
        horizontalPadding: 20, gradientTransitionRatio: 0.1
    };

    const { keyToColor, allKeys } = prepareColorMap(analyzedSpan);
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0) return;

    const mainSpanObject = { text: analyzedSpan.text, id: 'main-anchor' };
    const Renderer = window.wordTree.Renderer;

    Renderer.preCalculateAllNodeMetrics(mainSpanObject, true, config, svg);
    analyzedSpan.precedingAdjacencies.forEach(n => Renderer.preCalculateAllNodeMetrics(n, false, config, svg));
    analyzedSpan.followingAdjacencies.forEach(n => Renderer.preCalculateAllNodeMetrics(n, false, config, svg));

    const precedingResult = Renderer.calculateLayout(
        analyzedSpan.precedingAdjacencies, 0, 0, 0, -1, config);
    const followingResult = Renderer.calculateLayout(
        analyzedSpan.followingAdjacencies, 0, 0, 0, 1, config);

    const totalHeight = Math.max(
        precedingResult.totalHeight,
        followingResult.totalHeight,
        mainSpanObject.dynamicHeight
    ) + config.vGap * 2;

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
        svg.setAttribute(
            'viewBox',
            `${minX - config.horizontalPadding} 0 ${naturalContentWidth} ${totalHeight}`
        );
        container.style.height = `${totalHeight * scaleFactor}px`;
    }

    const mainSpanY = totalHeight / 2;
    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    Renderer.drawNodesAndConnectors(
        svg, analyzedSpan.precedingAdjacencies, mainSpanObject, 0, mainSpanY,
        -1, config, keyToColor, allKeys, container.id
    );
    Renderer.drawNodesAndConnectors(
        svg, analyzedSpan.followingAdjacencies, mainSpanObject, 0, mainSpanY,
        1, config, keyToColor, allKeys, container.id
    );
    Renderer.createNode(svg, mainSpanObject, 0, mainSpanY, false, config, keyToColor, container.id);
}

// === Color mapping helper ===
function prepareColorMap(analyzedSpan) {
    const allKeys = new Set();
    const gather = node => {
        if (!node || !node.sourceOccurrenceKeys) return;
        node.sourceOccurrenceKeys.forEach(k => allKeys.add(k));
        node.children?.forEach(gather);
    };
    analyzedSpan.precedingAdjacencies.forEach(gather);
    analyzedSpan.followingAdjacencies.forEach(gather);

    const keyToColor = new Map();
    const cardColors = analyzedSpan.cardColors || {};

    allKeys.forEach(k => {
        const name = k.substring(0, k.indexOf('['));
        keyToColor.set(k, cardColors[name] || '#dddddd');
    });
    return { keyToColor, allKeys };
}

// === New: per‐container spinner wrapper ===
function renderTreeWithSpinner(containerId, spinnerId, analyzedSpan) {
    setupGlobalEventHandlers();

    const container = document.getElementById(containerId);
    const spinner = document.getElementById(spinnerId);
    if (!container) return;
    if (spinner) spinner.style.display = 'block';

    if (wordTreeObservers.has(containerId)) {
        recalculateAndDraw(container);
        if (spinner) spinner.style.display = 'none';
        return;
    }

    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    container.appendChild(svg);

    const card = container.closest('.span-trees-card');
    if (card) card.__data = analyzedSpan;

    const resizeObserver = new ResizeObserver(() => recalculateAndDraw(container));
    resizeObserver.observe(container);
    wordTreeObservers.set(containerId, { observer: resizeObserver, animationFrameId: null });

    recalculateAndDraw(container);
    if (spinner) spinner.style.display = 'none';
}