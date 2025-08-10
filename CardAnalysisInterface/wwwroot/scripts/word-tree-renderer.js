// word-tree-renderer.ts
// This file contains all the logic for calculating layout and drawing the SVG.
// It is self-contained and does not handle events or animation.
export var WordTree;
(function (WordTree) {
    var Renderer;
    (function (Renderer) {
        /**
         * EXPORTED: Generates SVG <stop> elements for a gradient. This is now
         * exported to be used for dynamic gradient updates on hover.
         */
        function createGradientStops(keys, keyToPaletteMap, colorProperty, // 'hexLight' has been removed
        transitionRatio) {
            const numKeys = keys.length;
            if (numKeys === 0)
                return '';
            if (numKeys === 1) {
                const palette = keyToPaletteMap.get(keys[0]);
                const color = palette ? palette[colorProperty] : '#ccc';
                return `<stop offset="0%" stop-color="${color}" /><stop offset="100%" stop-color="${color}" />`;
            }
            const clampedRatio = Math.max(0, Math.min(1, transitionRatio));
            const transitionZoneWidth = (1 / numKeys) * clampedRatio;
            const halfTransition = transitionZoneWidth / 2;
            let stopsHtml = '';
            keys.forEach((key, i) => {
                const palette = keyToPaletteMap.get(key);
                const color = palette ? palette[colorProperty] : '#ccc';
                const bandStart = i / numKeys;
                const bandEnd = (i + 1) / numKeys;
                const solidStartOffset = (i === 0) ? bandStart : bandStart + halfTransition;
                const solidEndOffset = (i === numKeys - 1) ? bandEnd : bandEnd - halfTransition;
                stopsHtml += `<stop offset="${solidStartOffset * 100}%" stop-color="${color}" />`;
                stopsHtml += `<stop offset="${solidEndOffset * 100}%" stop-color="${color}" />`;
            });
            return stopsHtml;
        }
        Renderer.createGradientStops = createGradientStops;
        function preCalculateAllNodeMetrics(node, isAnchor, config, svg) {
            if (!node)
                return;
            const metrics = getNodeMetrics(node.text, isAnchor, config, svg);
            node.dynamicHeight = metrics.dynamicHeight;
            node.wrappedLines = metrics.wrappedLines;
            node.lineHeight = metrics.lineHeight;
            if (node.children)
                node.children.forEach((child) => preCalculateAllNodeMetrics(child, false, config, svg));
        }
        Renderer.preCalculateAllNodeMetrics = preCalculateAllNodeMetrics;
        function getNodeMetrics(text, isAnchor, config, svg) {
            const nodeText = String(text || '');
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
                }
                else {
                    currentLine = testLine;
                }
            }
            wrappedLines.push(currentLine);
            svg.removeChild(tempText);
            const totalTextHeight = wrappedLines.length * lineHeight;
            const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + padding * 2);
            return { dynamicHeight, wrappedLines, lineHeight };
        }
        Renderer.getNodeMetrics = getNodeMetrics;
        function calculateLayout(nodes, depth, parentX, parentY, direction, config) {
            if (!nodes || nodes.length === 0)
                return { layout: [], totalHeight: 0 };
            const layoutInfo = [];
            const nodeMetrics = [];
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
        Renderer.calculateLayout = calculateLayout;
        function drawNodesAndConnectors(svg, nodes, parentData, parentX, parentY, direction, config, keyToPaletteMap, allKeys, containerId) {
            if (!nodes)
                return;
            for (const node of nodes) {
                const { x: nodeX, y: nodeY } = node.layout;
                createRoundedConnector(svg, parentData, node, parentX, parentY, nodeX, nodeY, direction, config, keyToPaletteMap, allKeys, containerId);
                createNode(svg, node, nodeX, nodeY, true, config, keyToPaletteMap, containerId);
                if (node.children) {
                    drawNodesAndConnectors(svg, node.children, node, nodeX, nodeY, direction, config, keyToPaletteMap, allKeys, containerId);
                }
            }
        }
        Renderer.drawNodesAndConnectors = drawNodesAndConnectors;
        function createNode(svg, nodeData, cx, cy, isAdjacencyNode, config, keyToPaletteMap, containerId) {
            const { dynamicHeight, wrappedLines, lineHeight } = nodeData;
            const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
            group.setAttribute('class', 'node-group');
            if (isAdjacencyNode) {
                group.id = `group-node-${containerId}-${nodeData.id}`;
            }
            else {
                group.id = `group-node-${containerId}-main-anchor`;
            }
            const nodeWidth = isAdjacencyNode ? config.nodeWidth : config.mainSpanWidth;
            const baseShape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            baseShape.setAttribute('class', 'node-shape base-layer');
            baseShape.setAttribute('x', `${-nodeWidth / 2}`);
            baseShape.setAttribute('y', `${-dynamicHeight / 2}`);
            baseShape.setAttribute('width', `${nodeWidth}`);
            baseShape.setAttribute('height', `${dynamicHeight}`);
            baseShape.setAttribute('rx', "8");
            const highlightShape = baseShape.cloneNode();
            highlightShape.classList.remove('base-layer');
            highlightShape.setAttribute('class', 'highlight-overlay');
            group.appendChild(baseShape);
            group.appendChild(highlightShape);
            if (isAdjacencyNode) {
                group.dataset.sourceKeys = JSON.stringify(nodeData.sourceOccurrenceKeys || []);
                const keys = nodeData.sourceOccurrenceKeys || [];
                if (keys.length > 0) {
                    const defs = svg.querySelector('defs');
                    if (defs) {
                        const baseGradientId = `grad-node-base-${containerId}-${nodeData.id}`;
                        const baseGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                        baseGradient.setAttribute('id', baseGradientId);
                        baseGradient.innerHTML = createGradientStops(keys, keyToPaletteMap, 'hex', config.gradientTransitionRatio);
                        defs.appendChild(baseGradient);
                        baseShape.style.stroke = `url(#${baseGradientId})`;
                        const highlightGradientId = `grad-node-highlight-${containerId}-${nodeData.id}`;
                        const highlightGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                        highlightGradient.setAttribute('id', highlightGradientId);
                        // UPDATED: Use 'hexSat' for the initial render.
                        highlightGradient.innerHTML = createGradientStops(keys, keyToPaletteMap, 'hexSat', config.gradientTransitionRatio);
                        defs.appendChild(highlightGradient);
                        highlightShape.style.stroke = `url(#${highlightGradientId})`;
                    }
                }
            }
            else {
                group.classList.add('anchor-node-group');
                baseShape.style.fill = config.mainSpanFill;
                baseShape.style.setProperty('--node-border-color', config.mainSpanColor);
            }
            const textEl = document.createElementNS("http://www.w3.org/2000/svg", "text");
            textEl.setAttribute('class', 'node-text');
            if (!isAdjacencyNode) {
                textEl.style.fontSize = `${config.mainSpanFontSize}px`;
                textEl.style.fontWeight = 'bold';
            }
            const totalTextHeight = wrappedLines.length * lineHeight;
            const startY = -totalTextHeight / 2 + lineHeight * 0.8;
            wrappedLines.forEach((line, i) => {
                const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
                tspan.setAttribute('x', '0');
                tspan.setAttribute('dy', i === 0 ? `${startY}` : `${lineHeight}`);
                tspan.textContent = line;
                textEl.appendChild(tspan);
            });
            group.appendChild(textEl);
            group.setAttribute('transform', `translate(${cx}, ${cy})`);
            svg.appendChild(group);
        }
        Renderer.createNode = createNode;
        function createRoundedConnector(svg, parentData, childData, x1, y1, x2, y2, direction, config, keyToPaletteMap, allKeys, containerId) {
            const parentWidth = parentData.id === 'main-anchor' ? config.mainSpanWidth : config.nodeWidth;
            const startX = x1 + (direction * parentWidth / 2);
            const endX = x2 - (direction * config.nodeWidth / 2);
            const midX = (startX + endX) / 2;
            const r = config.cornerRadius;
            const verticalOffset = Math.abs(y2 - y1);
            let d;
            if (verticalOffset < 1e-6) {
                d = `M ${startX} ${y1} L ${endX} ${y2}`;
            }
            else if (verticalOffset < r * 2) {
                const smallR = verticalOffset / 2;
                const ySign = Math.sign(y2 - y1);
                const sweepFlag1 = direction * ySign > 0 ? 1 : 0;
                const sweepFlag2 = direction * ySign > 0 ? 0 : 1;
                d = `M ${startX} ${y1} L ${midX - smallR * direction} ${y1} A ${smallR} ${smallR} 0 0 ${sweepFlag1} ${midX} ${y1 + smallR * ySign} A ${smallR} ${smallR} 0 0 ${sweepFlag2} ${midX + smallR * direction} ${y2} L ${endX} ${y2}`;
            }
            else {
                const ySign = Math.sign(y2 - y1);
                const sweepFlag1 = direction * ySign > 0 ? 1 : 0;
                const sweepFlag2 = direction * ySign > 0 ? 0 : 1;
                d = `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${sweepFlag1} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${sweepFlag2} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
            }
            const parentKeys = parentData.id === 'main-anchor' ? allKeys : (parentData.sourceKeysSet || new Set());
            const childKeys = childData.sourceKeysSet || new Set();
            const commonKeys = [...childKeys].filter(key => parentKeys.has(key));
            const connectorGroup = document.createElementNS("http://www.w3.org/2000/svg", "g");
            connectorGroup.dataset.sourceKeys = JSON.stringify(commonKeys);
            connectorGroup.id = `group-conn-${containerId}-${childData.id}`;
            const basePath = document.createElementNS("http://www.w3.org/2000/svg", "path");
            basePath.setAttribute('class', 'connector-path base-layer');
            basePath.setAttribute('d', d);
            const highlightPath = basePath.cloneNode();
            highlightPath.classList.remove('base-layer');
            highlightPath.setAttribute('class', 'highlight-overlay');
            if (commonKeys.length > 0) {
                const defs = svg.querySelector('defs');
                if (defs) {
                    const idSuffix = `${containerId}-${childData.id}`;
                    const baseGradientId = `grad-conn-base-${idSuffix}`;
                    const highlightGradientId = `grad-conn-highlight-${idSuffix}`;
                    const createGradient = (id, colorProp) => {
                        const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                        gradient.setAttribute('id', id);
                        gradient.setAttribute('gradientUnits', 'userSpaceOnUse');
                        const deltaX = endX - startX;
                        const deltaY = y2 - y1;
                        if (Math.abs(deltaY) > Math.abs(deltaX)) {
                            gradient.setAttribute('x1', '0');
                            gradient.setAttribute('y1', `${y1}`);
                            gradient.setAttribute('x2', '0');
                            gradient.setAttribute('y2', `${y2}`);
                        }
                        else {
                            gradient.setAttribute('x1', `${startX}`);
                            gradient.setAttribute('y1', '0');
                            gradient.setAttribute('x2', `${endX}`);
                            gradient.setAttribute('y2', '0');
                        }
                        const reversedKeys = [...commonKeys].reverse();
                        gradient.innerHTML = createGradientStops(reversedKeys, keyToPaletteMap, colorProp, config.gradientTransitionRatio);
                        return gradient;
                    };
                    if (!defs.querySelector(`#${baseGradientId}`)) {
                        defs.appendChild(createGradient(baseGradientId, 'hex'));
                    }
                    if (!defs.querySelector(`#${highlightGradientId}`)) {
                        // UPDATED: Use 'hexSat' for the initial render.
                        defs.appendChild(createGradient(highlightGradientId, 'hexSat'));
                    }
                    basePath.style.stroke = `url(#${baseGradientId})`;
                    highlightPath.style.stroke = `url(#${highlightGradientId})`;
                }
            }
            connectorGroup.appendChild(basePath);
            connectorGroup.appendChild(highlightPath);
            svg.insertBefore(connectorGroup, svg.firstChild);
        }
        Renderer.createRoundedConnector = createRoundedConnector;
    })(Renderer = WordTree.Renderer || (WordTree.Renderer = {}));
})(WordTree || (WordTree = {}));
//# sourceMappingURL=word-tree-renderer.js.map