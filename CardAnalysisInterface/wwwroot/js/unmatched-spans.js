// A global map to store references for cleanup.
const wordTreeObservers = new Map();

/**
 * Entry point for rendering a word tree. Sets up a ResizeObserver to handle redraws.
 */
function renderTree(containerId, analyzedSpan) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error(`Word Tree Error: Container with ID '${containerId}' not found.`);
        return;
    }
    container.__data = analyzedSpan; // Store data for redraws

    if (wordTreeObservers.has(containerId)) {
        recalculateAndDraw(container);
        return;
    }

    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    container.appendChild(svg);

    const resizeObserver = new ResizeObserver(entries => {
        if (entries && entries.length > 0) {
            recalculateAndDraw(container);
        }
    });

    resizeObserver.observe(container);
    wordTreeObservers.set(containerId, resizeObserver);
    recalculateAndDraw(container);
}

/**
 * Disconnects the ResizeObserver for a specific tree to prevent memory leaks.
 */
function disposeTree(containerId) {
    if (wordTreeObservers.has(containerId)) {
        const observer = wordTreeObservers.get(containerId);
        observer.disconnect();
        wordTreeObservers.delete(containerId);
    }
}

/**
 * This function performs all calculations and DOM manipulations for drawing the tree.
 * It is designed to be called repeatedly on resize.
 */
function recalculateAndDraw(container) {
    const analyzedSpan = container.__data;
    const svg = container.querySelector('svg');
    if (!analyzedSpan || !svg) return;

    svg.innerHTML = '<defs></defs>';

    const config = {
        nodeWidth: 200, nodePadding: 8, mainSpanPadding: 12, nodeHeight: 40,
        hGap: 40, vGap: 20, cornerRadius: 10, blendPercentage: 1,
        mainSpanWidth: 220, mainSpanFontSize: 14, mainSpanLineHeight: 16,
        mainSpanFill: '#3a3a3a', mainSpanColor: "#e0e0e0",
        horizontalPadding: 20
    };

    const { keyToColor, allKeys } = prepareColorMap(analyzedSpan);
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0) return; // Don't render if the container isn't visible

    const mainSpanObject = { text: analyzedSpan.text, id: 'main-anchor', segments: [{ Text: analyzedSpan.text, TokenType: null }] };
    preCalculateAllNodeMetrics(mainSpanObject, true, config, svg);
    analyzedSpan.precedingAdjacencies.forEach(node => preCalculateAllNodeMetrics(node, false, config, svg));
    analyzedSpan.followingAdjacencies.forEach(node => preCalculateAllNodeMetrics(node, false, config, svg));

    const mainSpanX = 0;
    const precedingResult = calculateLayout(analyzedSpan.precedingAdjacencies, 0, mainSpanX, 0, -1, config);
    const followingResult = calculateLayout(analyzedSpan.followingAdjacencies, 0, mainSpanX, 0, 1, config);

    // --- Natural Size Calculation ---
    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    let minX = mainSpanX - config.mainSpanWidth / 2;
    let maxX = mainSpanX + config.mainSpanWidth / 2;
    const allLayoutNodes = [...precedingResult.layout, ...followingResult.layout];
    allLayoutNodes.forEach(node => {
        minX = Math.min(minX, node.layout.x - config.nodeWidth / 2);
        maxX = Math.max(maxX, node.layout.x + config.nodeWidth / 2);
    });
    const naturalTreeWidth = maxX - minX;
    const naturalContentWidth = naturalTreeWidth + config.horizontalPadding * 2;

    // --- NEW: Scaling and Sizing Logic ---
    if (naturalContentWidth <= availableWidth) {
        // CASE 1: The tree fits. Render at 1:1, centered. NO SCALING UP.
        const margin = (availableWidth - naturalTreeWidth) / 2;
        svg.setAttribute('viewBox', `${minX - margin} 0 ${availableWidth} ${totalHeight}`);
        container.style.height = `${totalHeight}px`;
    } else {
        // CASE 2: The tree overflows. Scale it DOWN.
        const scaleFactor = availableWidth / naturalContentWidth;
        const newHeight = totalHeight * scaleFactor;
        svg.setAttribute('viewBox', `${minX - config.horizontalPadding} 0 ${naturalContentWidth} ${totalHeight}`);
        container.style.height = `${newHeight}px`;
    }

    // --- Drawing ---
    const mainSpanY = totalHeight / 2;
    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    drawNodesAndConnectors(svg, analyzedSpan.precedingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, -1, config, keyToColor, allKeys);
    drawNodesAndConnectors(svg, analyzedSpan.followingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, 1, config, keyToColor, allKeys);
    createNode(svg, mainSpanObject, mainSpanX, mainSpanY, false, config, keyToColor, container.id);
}

// ===============================================
// == HELPER FUNCTIONS (No major changes needed) ==
// ===============================================

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

function createNode(svg, nodeData, cx, cy, isAdjacencyNode, config, keyToColor, containerId) {
    const { dynamicHeight, wrappedLines, lineHeight } = nodeData;
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.setAttribute('class', 'node-group');
    const keys = isAdjacencyNode ? (nodeData.sourceOccurrenceKeys || []) : [];
    if (isAdjacencyNode) {
        group.dataset.sourceKeys = JSON.stringify(keys);
    }
    const nodeWidth = isAdjacencyNode ? config.nodeWidth : config.mainSpanWidth;
    const shape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    shape.setAttribute('class', 'node-shape');
    shape.setAttribute('x', -nodeWidth / 2);
    shape.setAttribute('y', -dynamicHeight / 2);
    shape.setAttribute('width', nodeWidth);
    shape.setAttribute('height', dynamicHeight);
    shape.setAttribute('rx', 8);
    const textEl = document.createElementNS("http://www.w3.org/2000/svg", "text");
    textEl.setAttribute('class', 'node-text');
    if (!isAdjacencyNode) {
        group.classList.add('anchor-node-group');
        shape.style.fill = config.mainSpanFill;
        shape.style.setProperty('--node-border-color', config.mainSpanColor);
        textEl.style.fontSize = `${config.mainSpanFontSize}px`;
        textEl.style.fontWeight = 'bold';
    } else {
        if (keys.length > 1) {
            const defs = svg.querySelector('defs');
            const gradientId = `grad-${containerId}-${nodeData.id}`;
            if (!defs.querySelector(`#${gradientId}`)) {
                const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                gradient.setAttribute('id', gradientId);
                const segmentWidth = 100 / keys.length;
                keys.forEach((key, i) => {
                    const color = keyToColor.get(key) || '#ccc';
                    gradient.innerHTML += `<stop offset="${(i * segmentWidth) + (i === 0 ? 0 : config.blendPercentage)}%" stop-color="${color}" /><stop offset="${((i + 1) * segmentWidth) - (i === keys.length - 1 ? 0 : config.blendPercentage)}%" stop-color="${color}" />`;
                });
                defs.appendChild(gradient);
            }
            shape.style.stroke = `url(#${gradientId})`;
            shape.style.strokeWidth = '2.5px';
        } else if (keys.length === 1) {
            shape.style.setProperty('--node-border-color', keyToColor.get(keys[0]) || '#ccc');
        }
    }
    group.appendChild(shape);
    const totalTextHeight = wrappedLines.length * lineHeight;
    let startY = -totalTextHeight / 2 + lineHeight * 0.8;
    wrappedLines.forEach((line, i) => {
        const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tspan.setAttribute('x', 0);
        tspan.setAttribute('dy', i === 0 ? startY : lineHeight);
        tspan.textContent = line;
        textEl.appendChild(tspan);
    });
    group.appendChild(textEl);
    group.setAttribute('transform', `translate(${cx}, ${cy})`);
    if (isAdjacencyNode && keys.length > 0) {
        group.addEventListener('mouseover', () => {
            svg.classList.add('is-highlighting');
            const hoveredKeys = new Set(keys);
            svg.querySelectorAll('[data-source-keys]').forEach(el => {
                const elKeys = JSON.parse(el.dataset.sourceKeys);
                if (elKeys.some(key => hoveredKeys.has(key))) {
                    el.classList.add('highlight');
                }
            });
        });
        group.addEventListener('mouseout', () => {
            svg.classList.remove('is-highlighting');
            svg.querySelectorAll('.highlight').forEach(el => el.classList.remove('highlight'));
        });
    }
    svg.appendChild(group);
}

function drawNodesAndConnectors(svg, nodes, parentData, parentX, parentY, direction, config, keyToColor, allKeys) {
    if (!nodes) return;
    for (const node of nodes) {
        const { x: nodeX, y: nodeY } = node.layout;
        createRoundedConnector(svg, parentData, node, parentX, parentY, nodeX, nodeY, direction, config, keyToColor, allKeys);
        createNode(svg, node, nodeX, nodeY, true, config, keyToColor, svg.parentNode.id);
        drawNodesAndConnectors(svg, node.children, node, nodeX, nodeY, direction, config, keyToColor, allKeys);
    }
}

function createRoundedConnector(svg, parentData, childData, x1, y1, x2, y2, direction, config, keyToColor, allKeys) {
    const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
    const parentWidth = parentData.id === 'main-anchor' ? config.mainSpanWidth : config.nodeWidth;
    const startX = x1 + (direction * parentWidth / 2);
    const endX = x2 - (direction * config.nodeWidth / 2);
    const midX = (startX + endX) / 2;
    const r = config.cornerRadius;
    const ySign = Math.sign(y2 - y1);
    const d = (ySign === 0) ? `M ${startX} ${y1} L ${endX} ${y2}` : `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 1 : 0} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 0 : 1} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
    path.setAttribute('d', d.trim());
    const parentIsAnchor = parentData.id === 'main-anchor';
    const parentKeys = parentIsAnchor ? allKeys : new Set(parentData.sourceOccurrenceKeys || []);
    const childKeys = new Set(childData.sourceOccurrenceKeys || []);
    const commonKeys = [...childKeys].filter(key => parentKeys.has(key));
    path.dataset.sourceKeys = JSON.stringify(commonKeys);
    if (commonKeys.length > 0) {
        path.setAttribute('class', 'connector-path');
        if (commonKeys.length === 1) {
            path.style.setProperty('--node-border-color', keyToColor.get(commonKeys[0]) || '#ccc');
        }
    } else {
        path.setAttribute('class', 'connector-path no-common-key');
    }
    svg.insertBefore(path, svg.firstChild);
}