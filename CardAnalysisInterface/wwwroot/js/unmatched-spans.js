// A global map to store references for cleanup and animation frames.
const wordTreeObservers = new Map();

// --- JAVASCRIPT ANIMATION ENGINE ---
const animationConfig = {
    duration: 100, // The one true speed control, in milliseconds
    lowlightOpacity: 0.15
};

// Linear Interpolation (lerp) helper
function lerp(start, end, amount) {
    return start * (1 - amount) + end * amount;
}

/**
 * The core animation function. It manually animates the opacity of elements
 * over a set duration, guaranteeing synchronized movement.
 * @param {Map<HTMLElement, {start: number, end: number}>} elementsToAnimate - A map where keys are elements and values are their start/end opacities.
 * @param {string} containerId - The ID of the container to manage the animation frame.
 */
function animateOpacity(elementsToAnimate, containerId) {
    let observerData = wordTreeObservers.get(containerId);
    if (!observerData) return;

    // Cancel any ongoing animation for this container to prevent conflicts.
    if (observerData.animationFrameId) {
        cancelAnimationFrame(observerData.animationFrameId);
    }

    const startTime = performance.now();

    function animationStep(now) {
        const elapsed = now - startTime;
        const progress = Math.min(elapsed / animationConfig.duration, 1);

        elementsToAnimate.forEach((targets, element) => {
            const currentOpacity = lerp(targets.start, targets.end, progress);
            element.style.opacity = currentOpacity;
        });

        if (progress < 1) {
            observerData.animationFrameId = requestAnimationFrame(animationStep);
        } else {
            // Animation complete, ensure final state is set perfectly and clear the frame ID.
            elementsToAnimate.forEach((targets, element) => {
                element.style.opacity = targets.end;
            });
            observerData.animationFrameId = null;
        }
    }

    observerData.animationFrameId = requestAnimationFrame(animationStep);
}


// --- SETUP & DRAWING LOGIC ---

function renderTree(containerId, analyzedSpan) {
    const container = document.getElementById(containerId);
    if (!container) { console.error(`Word Tree Error: Container with ID '${containerId}' not found.`); return; }
    container.__data = analyzedSpan;

    if (wordTreeObservers.has(containerId)) {
        recalculateAndDraw(container);
        return;
    }

    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    container.appendChild(svg);
    const resizeObserver = new ResizeObserver(() => recalculateAndDraw(container));
    resizeObserver.observe(container);
    // Store both observer and a placeholder for the animation frame ID
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
    preCalculateAllNodeMetrics(mainSpanObject, true, config, svg);
    analyzedSpan.precedingAdjacencies.forEach(node => preCalculateAllNodeMetrics(node, false, config, svg));
    analyzedSpan.followingAdjacencies.forEach(node => preCalculateAllNodeMetrics(node, false, config, svg));
    const mainSpanX = 0;
    const precedingResult = calculateLayout(analyzedSpan.precedingAdjacencies, 0, mainSpanX, 0, -1, config);
    const followingResult = calculateLayout(analyzedSpan.followingAdjacencies, 0, mainSpanX, 0, 1, config);
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

    drawNodesAndConnectors(svg, analyzedSpan.precedingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, -1, config, keyToColor, allKeys, container.id);
    drawNodesAndConnectors(svg, analyzedSpan.followingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, 1, config, keyToColor, allKeys, container.id);
    createNode(svg, mainSpanObject, mainSpanX, mainSpanY, false, config, keyToColor, container.id);

    setupEventListeners(svg, container.id);
}

function setupEventListeners(svg, containerId) {
    const allKeyedElements = Array.from(svg.querySelectorAll('[data-source-keys]'));

    // Attach all visual layers to each keyed element for quick access.
    allKeyedElements.forEach(el => {
        el.__layers = {
            base: el.querySelector('.base-layer') || el,
            highlight: el.querySelector('.highlight-overlay'),
            text: el.querySelector('.node-text')
        };
    });

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
                elementsToAnimate.set(layers.base, { start: parseFloat(layers.base.style.opacity) || 1, end: 1 });
                if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(layers.highlight.style.opacity) || 0, end: 1 });
                if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(layers.text.style.opacity) || 1, end: 1 });
            } else {
                elementsToAnimate.set(layers.base, { start: parseFloat(layers.base.style.opacity) || 1, end: animationConfig.lowlightOpacity });
                if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(layers.highlight.style.opacity) || 0, end: 0 });
                if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(layers.text.style.opacity) || 1, end: animationConfig.lowlightOpacity });
            }
        });
        animateOpacity(elementsToAnimate, containerId);
    });

    svg.addEventListener('mouseout', () => {
        const elementsToAnimate = new Map();
        allKeyedElements.forEach(el => {
            const layers = el.__layers;
            elementsToAnimate.set(layers.base, { start: parseFloat(layers.base.style.opacity) || 1, end: 1 });
            if (layers.highlight) elementsToAnimate.set(layers.highlight, { start: parseFloat(layers.highlight.style.opacity) || 0, end: 0 });
            if (layers.text) elementsToAnimate.set(layers.text, { start: parseFloat(layers.text.style.opacity) || 1, end: 1 });
        });
        animateOpacity(elementsToAnimate, containerId);
    });
}

function createNode(svg, nodeData, cx, cy, isAdjacencyNode, config, keyToColor, containerId) {
    const { dynamicHeight, wrappedLines, lineHeight } = nodeData;
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.setAttribute('class', 'node-group');
    if (isAdjacencyNode) {
        group.dataset.sourceKeys = JSON.stringify(nodeData.sourceOccurrenceKeys || []);
    } else {
        group.classList.add('anchor-node-group');
    }

    const nodeWidth = isAdjacencyNode ? config.nodeWidth : config.mainSpanWidth;
    const shape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    shape.setAttribute('class', 'node-shape base-layer');
    shape.setAttribute('x', -nodeWidth / 2); shape.setAttribute('y', -dynamicHeight / 2);
    shape.setAttribute('width', nodeWidth); shape.setAttribute('height', dynamicHeight); shape.setAttribute('rx', 8);

    const highlightShape = shape.cloneNode();
    highlightShape.classList.remove('base-layer');
    highlightShape.setAttribute('class', 'highlight-overlay');

    const textEl = document.createElementNS("http://www.w3.org/2000/svg", "text");
    textEl.setAttribute('class', 'node-text');

    if (!isAdjacencyNode) {
        shape.style.fill = config.mainSpanFill;
        shape.style.setProperty('--node-border-color', config.mainSpanColor);
        textEl.style.fontSize = `${config.mainSpanFontSize}px`;
        textEl.style.fontWeight = 'bold';
    } else {
        const keys = nodeData.sourceOccurrenceKeys || [];
        if (keys.length > 1) {
            const defs = svg.querySelector('defs');
            const gradientId = `grad-node-${containerId}-${nodeData.id}`;
            if (!defs.querySelector(`#${gradientId}`)) {
                const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                gradient.setAttribute('id', gradientId);
                keys.forEach((key, i) => {
                    const color = keyToColor.get(key) || '#ccc';
                    gradient.innerHTML += `<stop offset="${(i / keys.length) * 100}%" stop-color="${color}" /><stop offset="${((i + 1) / keys.length) * 100}%" stop-color="${color}" />`;
                });
                defs.appendChild(gradient);
            }
            shape.style.stroke = `url(#${gradientId})`;
        } else if (keys.length === 1) {
            shape.style.setProperty('--node-border-color', keyToColor.get(keys[0]) || '#ccc');
        }
    }

    group.appendChild(shape);
    group.appendChild(highlightShape);
    const totalTextHeight = wrappedLines.length * lineHeight;
    let startY = -totalTextHeight / 2 + lineHeight * 0.8;
    wrappedLines.forEach((line, i) => {
        const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tspan.setAttribute('x', 0); tspan.setAttribute('dy', i === 0 ? startY : lineHeight);
        tspan.textContent = line;
        textEl.appendChild(tspan);
    });
    group.appendChild(textEl);
    group.setAttribute('transform', `translate(${cx}, ${cy})`);
    svg.appendChild(group);
}

function createRoundedConnector(svg, parentData, childData, x1, y1, x2, y2, direction, config, keyToColor, allKeys, containerId) {
    const parentWidth = parentData.id === 'main-anchor' ? config.mainSpanWidth : config.nodeWidth;
    const startX = x1 + (direction * parentWidth / 2);
    const endX = x2 - (direction * config.nodeWidth / 2);
    const midX = (startX + endX) / 2;
    const r = config.cornerRadius;
    const ySign = Math.sign(y2 - y1);
    const d = (ySign === 0) ? `M ${startX} ${y1} L ${endX} ${y2}` : `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 1 : 0} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 0 : 1} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
    const parentIsAnchor = parentData.id === 'main-anchor';
    const parentKeys = parentIsAnchor ? allKeys : new Set(parentData.sourceOccurrenceKeys || []);
    const childKeys = new Set(childData.sourceOccurrenceKeys || []);
    const commonKeys = [...childKeys].filter(key => parentKeys.has(key));

    const connectorGroup = document.createElementNS("http://www.w3.org/2000/svg", "g");
    connectorGroup.dataset.sourceKeys = JSON.stringify(commonKeys);

    const basePath = document.createElementNS("http://www.w3.org/2000/svg", "path");
    basePath.setAttribute('class', 'connector-path base-layer');
    basePath.setAttribute('d', d);

    const highlightPath = basePath.cloneNode();
    highlightPath.classList.remove('base-layer');
    highlightPath.setAttribute('class', 'highlight-overlay');

    if (commonKeys.length > 1) {
        const defs = svg.querySelector('defs');
        const gradientId = `grad-connector-${containerId}-${childData.id}`;
        if (!defs.querySelector(`#${gradientId}`)) {
            const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
            gradient.setAttribute('id', gradientId);
            gradient.setAttribute('x1', '0%'); gradient.setAttribute('y1', '0%');
            gradient.setAttribute('x2', '100%'); gradient.setAttribute('y2', '0%');
            const reversedKeys = [...commonKeys].reverse();
            reversedKeys.forEach((key, i) => {
                const color = keyToColor.get(key) || '#ccc';
                gradient.innerHTML += `<stop offset="${(i / reversedKeys.length) * 100}%" stop-color="${color}" /><stop offset="${((i + 1) / reversedKeys.length) * 100}%" stop-color="${color}" />`;
            });
            defs.appendChild(gradient);
        }
        basePath.style.stroke = `url(#${gradientId})`;
    } else if (commonKeys.length === 1) {
        basePath.style.setProperty('--node-border-color', keyToColor.get(commonKeys[0]) || '#ccc');
    }

    connectorGroup.appendChild(basePath);
    connectorGroup.appendChild(highlightPath);
    svg.insertBefore(connectorGroup, svg.firstChild);
}

// THIS IS THE MISSING FUNCTION
function drawNodesAndConnectors(svg, nodes, parentData, parentX, parentY, direction, config, keyToColor, allKeys, containerId) {
    if (!nodes) return;
    for (const node of nodes) {
        const { x: nodeX, y: nodeY } = node.layout;
        createRoundedConnector(svg, parentData, node, parentX, parentY, nodeX, nodeY, direction, config, keyToColor, allKeys, containerId);
        createNode(svg, node, nodeX, nodeY, true, config, keyToColor, containerId);
        drawNodesAndConnectors(svg, node.children, node, nodeX, nodeY, direction, config, keyToColor, allKeys, containerId);
    }
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

function getNodeMetrics(text, isAnchor, config, svg) {
    const nodeText = text || '';
    const nodeWidth = isAnchor ? config.mainSpanWidth : config.nodeWidth;
    const padding = isAnchor ? config.mainSpanPadding : config.nodePadding;
    const fontSize = isAnchor ? config.mainSpanFontSize : 12;
    const fontWeight = isAnchor ? 'bold' : 'normal';
    const lineHeight = isAnchor ? config.mainSpanLineHeight : 14;
    const availableWidth = nodeWidth - padding * 2;
    const tempText = document.createElementNS("http://www.w3.org/2000/svg", "text");
    tempText.setAttribute('class', 'node-text');
    tempText.style.fontSize = `${fontSize}px`;
    tempText.style.fontWeight = fontWeight;
    const tempTspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
    tempText.appendChild(tempTspan);
    svg.appendChild(tempText);
    const words = nodeText.split(' ');
    let currentLine = '';
    const wrappedLines = [];
    for (const word of words) {
        const testLine = currentLine ? `${currentLine} ${word}` : word;
        tempTspan.textContent = testLine;
        if (tempTspan.getComputedTextLength() > availableWidth && currentLine) {
            wrappedLines.push(currentLine);
            currentLine = word;
        } else {
            currentLine = testLine;
        }
    }
    wrappedLines.push(currentLine);
    svg.removeChild(tempText);
    const totalTextHeight = wrappedLines.length * lineHeight;
    const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + padding * 2);
    return { dynamicHeight, wrappedLines, lineHeight };
}

function preCalculateAllNodeMetrics(node, isAnchor = false, config, svg) {
    if (!node) return;
    const metrics = getNodeMetrics(node.text, isAnchor, config, svg);
    node.dynamicHeight = metrics.dynamicHeight;
    node.wrappedLines = metrics.wrappedLines;
    node.lineHeight = metrics.lineHeight;
    if (node.children) node.children.forEach(child => preCalculateAllNodeMetrics(child, false, config, svg));
}

function calculateLayout(nodes, depth, parentX, parentY, direction, config) {
    if (!nodes || nodes.length === 0) return { layout: [], totalHeight: 0 };
    const layoutInfo = [], nodeMetrics = [];
    for (const node of nodes) {
        const childrenResult = calculateLayout(node.children, depth + 1, 0, 0, direction, config);
        node.childrenLayout = childrenResult.layout;
        const effectiveHeight = Math.max(node.dynamicHeight, childrenResult.totalHeight);
        nodeMetrics.push({ node, effectiveHeight });
    }
    const totalGroupHeight = nodeMetrics.reduce((sum, metric) => sum + metric.effectiveHeight, 0) + Math.max(0, nodes.length - 1) * config.vGap;
    let currentY = parentY - totalGroupHeight / 2;
    for (const metric of nodeMetrics) {
        const { node, effectiveHeight } = metric;
        const parentWidth = (depth === 0) ? config.mainSpanWidth : config.nodeWidth;
        const offset = (parentWidth / 2) + config.hGap + (config.nodeWidth / 2);
        const nodeX = parentX + (direction * offset);
        const nodeY = currentY + effectiveHeight / 2;
        node.layout = { x: nodeX, y: nodeY };
        layoutInfo.push(node);
        for (const childNode of node.childrenLayout) {
            childNode.layout.x += nodeX;
            childNode.layout.y += nodeY;
        }
        layoutInfo.push(...node.childrenLayout);
        currentY += effectiveHeight + config.vGap;
    }
    return { layout: layoutInfo, totalHeight: totalGroupHeight };
}