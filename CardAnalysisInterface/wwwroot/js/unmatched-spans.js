/**
 * Renders a word tree visualization within a specified container.
 * @param {string} containerId The ID of the DOM element to render the tree into.
 * @param {object} analyzedSpan The root data object, corresponding to the C# `AnalyzedUnmatchedSpan` record.
 *                 This object should have properties: `text`, `precedingAdjacencies`, `followingAdjacencies`.
 *                 The adjacency nodes should have `id`, `text`, `children`, `sourceOccurrenceKeys`, and `tokenTypeColor`.
 */
function renderTree(containerId, analyzedSpan) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error(`Word Tree Error: Container with ID '${containerId}' not found.`);
        return;
    }
    container.innerHTML = ''; // Clear previous renders

    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    svg.innerHTML = '<defs></defs>'; // For gradients
    container.appendChild(svg);

    // --- Configuration ---
    const config = {
        nodeWidth: 200,
        nodePadding: 8,
        mainSpanPadding: 12,
        nodeHeight: 40,
        hGap: 40,
        vGap: 20,
        cornerRadius: 10,
        blendPercentage: 1,
        mainSpanWidth: 220,
        mainSpanFontSize: 14,
        mainSpanLineHeight: 16,
        mainSpanFill: '#3a3a3a',
        mainSpanColor: "#e0e0e0"
    };

    const throughLineColors = [
        '#9b59b6', '#3498db', '#1abc9c', '#2ecc71', '#f1c40f', '#e67e22',
        '#8e44ad', '#2980b9', '#16a085', '#27ae60', '#f39c12', '#e74c3c'
    ];
    const colorIndexMap = [0, 2, 4, 6, 8, 10, 1, 3, 5, 7, 9, 11];

    // --- Data Preparation ---
    const allKeys = new Set();
    const processNodeForKeys = (node) => {
        if (!node) return;
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

    let { width } = container.getBoundingClientRect();

    // --- Helper Functions ---

    function getNodeMetrics(text, isAnchor) {
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

        const words = text.split(' ');
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
        const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + padding);

        return { dynamicHeight, wrappedLines, lineHeight };
    }

    function preCalculateAllNodeMetrics(node, isAnchor = false) {
        if (!node) return;
        const metrics = getNodeMetrics(node.text, isAnchor);
        node.dynamicHeight = metrics.dynamicHeight;
        node.wrappedLines = metrics.wrappedLines;
        node.lineHeight = metrics.lineHeight;

        if (node.children) node.children.forEach(child => preCalculateAllNodeMetrics(child, false));
    }

    function calculateLayout(nodes, depth, parentX, parentY, direction) {
        if (!nodes || nodes.length === 0) return { layout: [], totalHeight: 0 };
        const layoutInfo = [], nodeMetrics = [];
        for (const node of nodes) {
            const childrenResult = calculateLayout(node.children, depth + 1, 0, 0, direction);
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

    /**
     * Creates and appends a single node (either anchor or adjacency) to the SVG.
     * @param {object} nodeData - The data for the node (text, id, keys, etc.).
     * @param {number} cx - The center-x coordinate.
     * @param {number} cy - The center-y coordinate.
     * @param {boolean} isAdjacencyNode - True if this is a preceding/following node, false for the main anchor.
     */
    function createNode(nodeData, cx, cy, isAdjacencyNode) {
        const { dynamicHeight, wrappedLines, lineHeight, tokenTypeColor } = nodeData;
        const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
        group.setAttribute('class', 'node-group');

        // Get the source keys only for adjacency nodes. The anchor has no keys.
        const keys = isAdjacencyNode ? (nodeData.sourceOccurrenceKeys || []) : [];
        if (isAdjacencyNode) {
            // Store keys on the DOM element for the hover interaction logic.
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
            // This is the central anchor node.
            // Add a specific class so CSS can exclude it from lowlighting.
            group.classList.add('anchor-node-group');

            shape.style.fill = config.mainSpanFill;
            shape.style.setProperty('--node-border-color', config.mainSpanColor);
            textEl.style.fontSize = `${config.mainSpanFontSize}px`;
            textEl.style.fontWeight = 'bold';

        } else {
            // This is a preceding/following adjacency node.
            if (keys.length > 1) {
                // Create a multi-color gradient border for nodes in multiple "through-lines".
                const defs = svg.querySelector('defs');
                const gradientId = `grad-${containerId}-${nodeData.id}`;
                const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                gradient.setAttribute('id', gradientId);
                const segmentWidth = 100 / keys.length;
                keys.forEach((key, i) => {
                    const color = keyToColor.get(key) || '#ccc';
                    const solidStart = (i * segmentWidth) + (i === 0 ? 0 : config.blendPercentage);
                    const solidEnd = ((i + 1) * segmentWidth) - (i === keys.length - 1 ? 0 : config.blendPercentage);
                    gradient.innerHTML += `<stop offset="${solidStart}%" stop-color="${color}" /><stop offset="${solidEnd}%" stop-color="${color}" />`;
                });
                defs.appendChild(gradient);
                shape.style.stroke = `url(#${gradientId})`;
                shape.style.strokeWidth = '2.5px';
            } else if (keys.length === 1) {
                // Use a solid color border for nodes in a single "through-line".
                shape.style.setProperty('--node-border-color', keyToColor.get(keys[0]) || '#ccc');
            }

            // If the source token had a type (e.g., keyword), color its text.
            if (tokenTypeColor) {
                textEl.style.fill = tokenTypeColor;
                textEl.style.fontWeight = 'bold';
            }
        }
        group.appendChild(shape);

        // Render text with line wrapping.
        const totalTextHeight = wrappedLines.length * lineHeight;
        const startY = -totalTextHeight / 2 + lineHeight * 0.7;
        wrappedLines.forEach((line, i) => {
            const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
            tspan.setAttribute('x', 0);
            tspan.setAttribute('dy', i === 0 ? startY : lineHeight);
            tspan.textContent = line;
            textEl.appendChild(tspan);
        });
        group.appendChild(textEl);

        group.setAttribute('transform', `translate(${cx}, ${cy})`);

        // Add hover listeners only to interactive adjacency nodes.
        if (isAdjacencyNode && keys.length > 0) {
            group.addEventListener('mouseover', () => {
                svg.classList.add('is-highlighting');
                const hoveredKeys = new Set(keys);
                svg.querySelectorAll('[data-source-keys]').forEach(el => {
                    const elKeys = JSON.parse(el.dataset.sourceKeys);
                    // Highlight if there is any intersection of keys.
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

    function createRoundedConnector(parentData, childData, x1, y1, x2, y2, direction) {
        const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
        const parentWidth = parentData.id === 'main-anchor' ? config.mainSpanWidth : config.nodeWidth;
        const startX = x1 + (direction * parentWidth / 2);
        const endX = x2 - (direction * config.nodeWidth / 2);
        const midX = (startX + endX) / 2;
        const r = config.cornerRadius;
        const ySign = Math.sign(y2 - y1);
        const d = (ySign === 0)
            ? `M ${startX} ${y1} L ${endX} ${y2}`
            : `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 1 : 0} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 0 : 1} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
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

    function drawNodesAndConnectors(nodes, parentData, parentX, parentY, direction) {
        if (!nodes) return;
        for (const node of nodes) {
            const { x: nodeX, y: nodeY } = node.layout;
            createRoundedConnector(parentData, node, parentX, parentY, nodeX, nodeY, direction);
            createNode(node, nodeX, nodeY, true);
            drawNodesAndConnectors(node.children, node, nodeX, nodeY, direction);
        }
    }

    // --- Main Render Execution ---
    const mainSpanObject = { text: analyzedSpan.text, id: 'main-anchor' };
    preCalculateAllNodeMetrics(mainSpanObject, true);
    analyzedSpan.precedingAdjacencies.forEach(node => preCalculateAllNodeMetrics(node, false));
    analyzedSpan.followingAdjacencies.forEach(node => preCalculateAllNodeMetrics(node, false));

    const mainSpanX = width / 2;
    const precedingResult = calculateLayout(analyzedSpan.precedingAdjacencies, 0, mainSpanX, 0, -1);
    const followingResult = calculateLayout(analyzedSpan.followingAdjacencies, 0, mainSpanX, 0, 1);

    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    const mainSpanY = totalHeight / 2;

    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    container.style.height = `${totalHeight}px`;
    svg.setAttribute('viewBox', `0 0 ${width} ${totalHeight}`);

    drawNodesAndConnectors(analyzedSpan.precedingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, -1);
    drawNodesAndConnectors(analyzedSpan.followingAdjacencies, mainSpanObject, mainSpanX, mainSpanY, 1);
    createNode(mainSpanObject, mainSpanX, mainSpanY, false); // isAdjacencyNode = false
}