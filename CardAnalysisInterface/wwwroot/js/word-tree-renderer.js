// word-tree-renderer.js

// This file contains all the logic for calculating layout and drawing the SVG.
// It is self-contained and does not handle events or animation.

window.wordTree = window.wordTree || {};

window.wordTree.Renderer = {

    /**
     * Generates SVG <stop> elements for a gradient with blended transitions.
     * @private
     */
    _createGradientStops: function (keys, keyToColor, transitionRatio) {
        const numKeys = keys.length;
        const clampedRatio = Math.max(0, Math.min(1, transitionRatio));

        if (numKeys <= 1 || clampedRatio === 0) {
            // Fallback for simple cases (one color) or hard stops (ratio is 0)
            let stops = '';
            const keyList = numKeys > 0 ? keys : ['default'];
            const numSegments = keyList.length;
            keyList.forEach((key, i) => {
                const color = keyToColor.get(key) || '#ccc';
                stops += `<stop offset="${(i / numSegments) * 100}%" stop-color="${color}" /><stop offset="${((i + 1) / numSegments) * 100}%" stop-color="${color}" />`;
            });
            return stops;
        }

        const transitionZoneWidth = (1 / numKeys) * clampedRatio;
        const halfTransition = transitionZoneWidth / 2;
        let stopsHtml = '';

        keys.forEach((key, i) => {
            const color = keyToColor.get(key) || '#ccc';
            const bandStart = i / numKeys;
            const bandEnd = (i + 1) / numKeys;

            const solidStartOffset = (i === 0) ? bandStart : bandStart + halfTransition;
            const solidEndOffset = (i === numKeys - 1) ? bandEnd : bandEnd - halfTransition;

            stopsHtml += `<stop offset="${solidStartOffset * 100}%" stop-color="${color}" />`;
            stopsHtml += `<stop offset="${solidEndOffset * 100}%" stop-color="${color}" />`;
        });

        return stopsHtml;
    },

    preCalculateAllNodeMetrics: function (node, isAnchor, config, svg) {
        if (!node) return;
        const metrics = this.getNodeMetrics(node.text, isAnchor, config, svg);
        node.dynamicHeight = metrics.dynamicHeight;
        node.wrappedLines = metrics.wrappedLines;
        node.lineHeight = metrics.lineHeight;
        if (node.children) node.children.forEach(child => this.preCalculateAllNodeMetrics(child, false, config, svg));
    },

    getNodeMetrics: function (text, isAnchor, config, svg) {
        // --- THIS IS THE FIX ---
        // Ensure nodeText is always a string to prevent .split() errors on non-string types.
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
            } else {
                currentLine = testLine;
            }
        }
        wrappedLines.push(currentLine);
        svg.removeChild(tempText);
        const totalTextHeight = wrappedLines.length * lineHeight;
        const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + padding * 2);
        return { dynamicHeight, wrappedLines, lineHeight };
    },

    calculateLayout: function (nodes, depth, parentX, parentY, direction, config) {
        if (!nodes || nodes.length === 0) return { layout: [], totalHeight: 0 };
        const layoutInfo = [], nodeMetrics = [];
        for (const node of nodes) {
            const childrenResult = this.calculateLayout(node.children, depth + 1, 0, 0, direction, config);
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
    },

    drawNodesAndConnectors: function (svg, nodes, parentData, parentX, parentY, direction, config, keyToColor, allKeys, containerId) {
        if (!nodes) return;
        for (const node of nodes) {
            const { x: nodeX, y: nodeY } = node.layout;
            this.createRoundedConnector(svg, parentData, node, parentX, parentY, nodeX, nodeY, direction, config, keyToColor, allKeys, containerId);
            this.createNode(svg, node, nodeX, nodeY, true, config, keyToColor, containerId);
            this.drawNodesAndConnectors(svg, node.children, node, nodeX, nodeY, direction, config, keyToColor, allKeys, containerId);
        }
    },

    createNode: function (svg, nodeData, cx, cy, isAdjacencyNode, config, keyToColor, containerId) {
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
                    gradient.innerHTML = this._createGradientStops(keys, keyToColor, config.gradientTransitionRatio);
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
    },

    // --- MODIFIED FUNCTION ---
    createRoundedConnector: function (svg, parentData, childData, x1, y1, x2, y2, direction, config, keyToColor, allKeys, containerId) {
        const parentWidth = parentData.id === 'main-anchor' ? config.mainSpanWidth : config.nodeWidth;
        const startX = x1 + (direction * parentWidth / 2);
        const endX = x2 - (direction * config.nodeWidth / 2);
        const midX = (startX + endX) / 2;
        const r = config.cornerRadius;
        const verticalOffset = Math.abs(y2 - y1);

        let d;

        // Use a small epsilon for floating point comparisons to determine if the line is horizontal
        if (verticalOffset < 1e-6) {
            // Case 1: Perfectly horizontal line.
            d = `M ${startX} ${y1} L ${endX} ${y2}`;
        } else if (verticalOffset < r * 2) {
            // Case 2: The "heartbeat" artifact case.
            // Vertical offset is too small for full-radius curves. Use a smaller radius
            // equal to half the vertical offset, creating two smooth, connecting curves
            // with no vertical line segment between them.
            const smallR = verticalOffset / 2;
            const ySign = Math.sign(y2 - y1);
            const sweepFlag1 = direction * ySign > 0 ? 1 : 0;
            const sweepFlag2 = direction * ySign > 0 ? 0 : 1;
            d = `M ${startX} ${y1} L ${midX - smallR * direction} ${y1} A ${smallR} ${smallR} 0 0 ${sweepFlag1} ${midX} ${y1 + smallR * ySign} A ${smallR} ${smallR} 0 0 ${sweepFlag2} ${midX + smallR * direction} ${y2} L ${endX} ${y2}`;
        } else {
            // Case 3: The standard case with a full radius and a vertical segment.
            const ySign = Math.sign(y2 - y1);
            const sweepFlag1 = direction * ySign > 0 ? 1 : 0;
            const sweepFlag2 = direction * ySign > 0 ? 0 : 1;
            d = `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${sweepFlag1} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${sweepFlag2} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
        }

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
                gradient.setAttribute('gradientUnits', 'userSpaceOnUse');

                const deltaX = endX - startX;
                const deltaY = y2 - y1;

                if (Math.abs(deltaY) > Math.abs(deltaX)) {
                    gradient.setAttribute('x1', startX); gradient.setAttribute('y1', y1);
                    gradient.setAttribute('x2', startX); gradient.setAttribute('y2', y2);
                } else {
                    gradient.setAttribute('x1', startX); gradient.setAttribute('y1', y1);
                    gradient.setAttribute('x2', endX); gradient.setAttribute('y2', y1);
                }

                const reversedKeys = [...commonKeys].reverse();
                gradient.innerHTML = this._createGradientStops(reversedKeys, keyToColor, config.gradientTransitionRatio);
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
};