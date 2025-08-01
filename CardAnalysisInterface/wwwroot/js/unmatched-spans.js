// wwwroot/js/wordTree.js

function render(containerId, spanData) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error(`Word Tree Error: Container with ID '${containerId}' not found.`);
        return;
    }
    container.innerHTML = '';

    const svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    svg.innerHTML = '<defs></defs>';
    container.appendChild(svg);

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

    const nodeObjectToSentenceIndices = new Map();
    const allNodesById = new Map();
    let { width } = container.getBoundingClientRect();

    const mainSpanData = {
        id: spanData.id, text: spanData.text,
        preceding: spanData.preceding, following: spanData.following
    };
    const sentences = spanData.sentences;

    function indexAllNodes(node) {
        if (!node) return;
        allNodesById.set(node.id, node);
        if (node.children) node.children.forEach(indexAllNodes);
    }

    function mapObjectsToSentences() {
        nodeObjectToSentenceIndices.clear();
        allNodesById.clear();
        indexAllNodes(mainSpanData);
        if (mainSpanData.preceding) mainSpanData.preceding.forEach(indexAllNodes);
        if (mainSpanData.following) mainSpanData.following.forEach(indexAllNodes);
        const addNode = (nodeObj, sentenceIndex) => {
            if (!nodeObjectToSentenceIndices.has(nodeObj)) nodeObjectToSentenceIndices.set(nodeObj, []);
            nodeObjectToSentenceIndices.get(nodeObj).push(sentenceIndex);
        };
        sentences.forEach((sentencePath, index) => {
            addNode(mainSpanData, index);
            sentencePath.forEach(nodeId => {
                const nodeObj = allNodesById.get(nodeId);
                if (nodeObj) addNode(nodeObj, index);
            });
        });
    }

    // --- NEW: Accurate measurement using getComputedTextLength() ---
    function getNodeMetrics(text, isAnchor) {
        const nodeWidth = isAnchor ? config.mainSpanWidth : config.nodeWidth;
        const padding = isAnchor ? config.mainSpanPadding : config.nodePadding;
        const fontSize = isAnchor ? config.mainSpanFontSize : 12;
        const fontWeight = isAnchor ? 'bold' : 'normal';
        const lineHeight = isAnchor ? config.mainSpanLineHeight : 14;
        const availableWidth = nodeWidth - padding * 2;

        const tempText = document.createElementNS("http://www.w3.org/2000/svg", "text");
        tempText.setAttribute('class', 'node-text'); // Apply basic styles
        tempText.style.fontSize = `${fontSize}px`;
        tempText.style.fontWeight = fontWeight;
        const tempTspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tempText.appendChild(tempTspan);
        svg.appendChild(tempText); // Add to DOM to be measurable

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

        svg.removeChild(tempText); // Clean up temp element

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

    function createNode(nodeData, cx, cy, isAdjacency) {
        const { tokenTypeColor, dynamicHeight, wrappedLines, lineHeight } = nodeData;
        const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
        group.setAttribute('class', 'node-group');
        const sentenceIndices = nodeObjectToSentenceIndices.get(nodeData) || [];
        sentenceIndices.forEach(idx => group.classList.add(`sentence-${idx}`));

        const nodeWidth = isAdjacency ? config.nodeWidth : config.mainSpanWidth;

        const shape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
        shape.setAttribute('class', 'node-shape');
        shape.setAttribute('x', -nodeWidth / 2);
        shape.setAttribute('y', -dynamicHeight / 2);
        shape.setAttribute('width', nodeWidth);
        shape.setAttribute('height', dynamicHeight);
        shape.setAttribute('rx', 8);

        const textEl = document.createElementNS("http://www.w3.org/2000/svg", "text");
        textEl.setAttribute('class', 'node-text');

        if (!isAdjacency) {
            shape.style.fill = config.mainSpanFill;
            shape.style.setProperty('--node-border-color', config.mainSpanColor);
            textEl.style.fontSize = `${config.mainSpanFontSize}px`;
            textEl.style.fontWeight = 'bold';
        } else {
            if (sentenceIndices.length > 1) {
                const defs = svg.querySelector('defs');
                const gradientId = `grad-node-${containerId}-${nodeData.id}`;
                const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                gradient.setAttribute('id', gradientId);
                const segmentWidth = 100 / sentenceIndices.length;
                sentenceIndices.forEach((sentenceIdx, i) => {
                    const colorIndex = colorIndexMap[sentenceIdx % colorIndexMap.length];
                    const color = throughLineColors[colorIndex];
                    const solidStart = (i * segmentWidth) + (i === 0 ? 0 : config.blendPercentage);
                    const solidEnd = ((i + 1) * segmentWidth) - (i === sentenceIndices.length - 1 ? 0 : config.blendPercentage);
                    gradient.innerHTML += `<stop offset="${solidStart}%" stop-color="${color}" /><stop offset="${solidEnd}%" stop-color="${color}" />`;
                });
                defs.appendChild(gradient);
                shape.style.stroke = `url(#${gradientId})`;
                shape.style.strokeWidth = '2.5px';
            } else if (sentenceIndices.length === 1) {
                const colorIndex = colorIndexMap[sentenceIndices[0] % colorIndexMap.length];
                shape.style.setProperty('--node-border-color', throughLineColors[colorIndex]);
            }
            if (tokenTypeColor) {
                textEl.style.fill = tokenTypeColor;
                textEl.style.fontWeight = 'bold';
            }
        }
        group.appendChild(shape);

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
        group.addEventListener('mouseover', () => {
            if (sentenceIndices.length > 0) {
                svg.classList.add('is-highlighting');
                sentenceIndices.forEach(idx => {
                    svg.querySelectorAll(`.sentence-${idx}`).forEach(el => el.classList.add('highlight'));
                });
            }
        });
        group.addEventListener('mouseout', () => {
            svg.classList.remove('is-highlighting');
            svg.querySelectorAll('.highlight').forEach(el => el.classList.remove('highlight'));
        });
        svg.appendChild(group);
    }

    function createRoundedConnector(parentData, childData, x1, y1, x2, y2, direction) {
        const path = document.createElementNS("http://www.w3.org/2000/svg", "path");
        const parentWidth = parentData.id === mainSpanData.id ? config.mainSpanWidth : config.nodeWidth;
        const startX = x1 + (direction * parentWidth / 2);
        const endX = x2 - (direction * config.nodeWidth / 2);
        const midX = (startX + endX) / 2;
        const r = config.cornerRadius;
        const ySign = Math.sign(y2 - y1);
        const d = (ySign === 0)
            ? `M ${startX} ${y1} L ${endX} ${y2}`
            : `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 1 : 0} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${direction * ySign > 0 ? 0 : 1} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
        path.setAttribute('d', d.trim());
        path.setAttribute('class', 'connector-path');
        const parentSentences = new Set(nodeObjectToSentenceIndices.get(parentData) || []);
        const commonSentenceIndices = (nodeObjectToSentenceIndices.get(childData) || []).filter(idx => parentSentences.has(idx));
        commonSentenceIndices.forEach(idx => path.classList.add(`sentence-${idx}`));
        if (commonSentenceIndices.length === 1) {
            const colorIndex = colorIndexMap[commonSentenceIndices[0] % colorIndexMap.length];
            path.style.setProperty('--node-border-color', throughLineColors[colorIndex]);
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
    mapObjectsToSentences();

    preCalculateAllNodeMetrics(mainSpanData, true);
    if (mainSpanData.preceding) mainSpanData.preceding.forEach(node => preCalculateAllNodeMetrics(node, false));
    if (mainSpanData.following) mainSpanData.following.forEach(node => preCalculateAllNodeMetrics(node, false));

    const mainSpanX = width / 2;
    const precedingResult = calculateLayout(mainSpanData.preceding, 0, mainSpanX, 0, -1);
    const followingResult = calculateLayout(mainSpanData.following, 0, mainSpanX, 0, 1);

    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanData.dynamicHeight) + config.vGap * 2;
    const mainSpanY = totalHeight / 2;

    precedingResult.layout.forEach(n => n.layout.y += mainSpanY);
    followingResult.layout.forEach(n => n.layout.y += mainSpanY);

    container.style.height = `${totalHeight}px`;
    svg.setAttribute('viewBox', `0 0 ${width} ${totalHeight}`);

    drawNodesAndConnectors(mainSpanData.preceding, mainSpanData, mainSpanX, mainSpanY, -1);
    drawNodesAndConnectors(mainSpanData.following, mainSpanData, mainSpanX, mainSpanY, 1);
    createNode(mainSpanData, mainSpanX, mainSpanY, false);
}