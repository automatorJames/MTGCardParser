// word-tree-renderer.ts
// This file contains all the logic for calculating layout and drawing the SVG.
// It is self-contained and does not handle events or animation.
export var WordTree;
(function (WordTree) {
    var Renderer;
    (function (Renderer) {
        // ---- Internal metadata (no changes to AdjacencyNode interface required) ----
        const columnIndexMap = new WeakMap(); // 1-based from anchor
        const fanDeltaMap = new WeakMap(); // δ: extra final-horizontal length used to fan
        function createGradientStops(keys, keyToPaletteMap, colorProperty, transitionRatio) {
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
        function preCalculateAllNodeMetrics(node, _isAnchor, config, svg) {
            if (!node)
                return;
            const metrics = getNodeMetrics(node.text, config, svg);
            node.dynamicHeight = metrics.dynamicHeight;
            node.wrappedLines = metrics.wrappedLines;
            node.lineHeight = metrics.lineHeight;
            if (node.children)
                node.children.forEach((child) => preCalculateAllNodeMetrics(child, false, config, svg));
        }
        Renderer.preCalculateAllNodeMetrics = preCalculateAllNodeMetrics;
        function getNodeMetrics(text, config, svg) {
            const nodeText = String(text || '');
            const nodeWidth = config.nodeWidth; // same for all nodes (anchor included)
            const padding = config.nodePadding; // same padding
            const fontSize = 12; // same font size
            const fontWeight = 'normal'; // same weight
            const lineHeight = 14; // same line height
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
                const parentWidth = config.nodeWidth; // anchor uses same width as others
                const offset = (parentWidth / 2) + config.hGap + (config.nodeWidth / 2);
                const nodeX = parentX + (direction * offset);
                const nodeY = currentY + effectiveHeight / 2;
                node.layout = { x: nodeX, y: nodeY };
                columnIndexMap.set(node, depth + 1); // 1-based column index
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
        /**
         * Assign per-connector fan deltas (δ) and compute per-column outward push.
         * Ordering: sort by |dy| ascending (nearest first); nearest gets δ=0, farthest gets (n-1)*fanGap.
         */
        function computeColumnOffsetsAndAssignFan(roots, anchorX, anchorY, direction, config) {
            const fanGap = Math.max(0, config.fanGap ?? 0);
            const columnStats = new Map();
            const noteStats = (col, upCount, downCount) => {
                const cur = columnStats.get(col) ?? { maxUp: 0, maxDown: 0 };
                if (upCount > cur.maxUp)
                    cur.maxUp = upCount;
                if (downCount > cur.maxDown)
                    cur.maxDown = downCount;
                columnStats.set(col, cur);
            };
            const processChildren = (children, parent) => {
                if (!children || children.length === 0)
                    return;
                const up = [];
                const down = [];
                const flat = [];
                for (const c of children) {
                    const dy = c.layout.y - parent.layout.y;
                    if (Math.abs(dy) < 1e-6)
                        flat.push(c);
                    else if (dy > 0)
                        down.push(c);
                    else
                        up.push(c);
                }
                const byAbsDy = (a, b) => Math.abs(a.layout.y - parent.layout.y) - Math.abs(b.layout.y - parent.layout.y);
                const assignGroup = (arr, kind) => {
                    if (arr.length === 0)
                        return;
                    // NEAREST first
                    arr.sort(byAbsDy);
                    const n = arr.length;
                    for (let i = 0; i < n; i++) {
                        const child = arr[i];
                        const delta = i * fanGap; // nearest δ=0 ... farthest δ=(n-1)*fanGap
                        fanDeltaMap.set(child, delta);
                    }
                    const colIndex = columnIndexMap.get(arr[0]) ?? 0;
                    if (kind === 'up')
                        noteStats(colIndex, n, 0);
                    else
                        noteStats(colIndex, 0, n);
                    for (const c of arr)
                        processChildren(c.children, c);
                };
                assignGroup(up, 'up');
                assignGroup(down, 'down');
                // same-row connectors: δ=0, then recurse
                for (const c of flat) {
                    fanDeltaMap.set(c, 0);
                    processChildren(c.children, c);
                }
            };
            const pseudoParent = { layout: { x: anchorX, y: anchorY }, children: roots };
            processChildren(roots, pseudoParent);
            // Per-column raw push in pixels (NOT cumulative): max(up, down) * fanGap
            const columnPushRaw = new Map();
            columnStats.forEach((v, col) => {
                const needed = fanGap * Math.max(v.maxUp, v.maxDown);
                columnPushRaw.set(col, needed);
            });
            return columnPushRaw;
        }
        Renderer.computeColumnOffsetsAndAssignFan = computeColumnOffsetsAndAssignFan;
        // Helper to access a node's column index without exposing internals
        function getColumnIndex(node) {
            return columnIndexMap.get(node) ?? 0;
        }
        Renderer.getColumnIndex = getColumnIndex;
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
            const nodeWidth = config.nodeWidth; // same for all nodes (anchor included)
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
                        highlightGradient.innerHTML = createGradientStops(keys, keyToPaletteMap, 'hexSat', config.gradientTransitionRatio);
                        defs.appendChild(highlightGradient);
                        highlightShape.style.stroke = `url(#${highlightGradientId})`;
                    }
                }
            }
            else {
                group.classList.add('main-anchor-span');
                baseShape.style.fill = config.mainSpanFill;
                baseShape.style.setProperty('--node-border-color', config.mainSpanColor);
            }
            const textEl = document.createElementNS("http://www.w3.org/2000/svg", "text");
            textEl.setAttribute('class', 'node-text');
            // Same styles for all nodes
            textEl.style.fontSize = `12px`;
            textEl.style.fontWeight = 'normal';
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
            const parentWidth = config.nodeWidth; // anchor uses same width as others
            const startX = x1 + (direction * parentWidth / 2);
            const endX = x2 - (direction * config.nodeWidth / 2);
            const ySign = Math.sign(y2 - y1) || 1;
            const verticalOffset = Math.abs(y2 - y1);
            // Total horizontal “budget” in direction of travel
            const H = direction * (endX - startX); // > 0
            // Per-connector fan delta (more delta => longer last horizontal, earlier branch from parent)
            const deltaRaw = fanDeltaMap.get(childData) || 0;
            const delta = Math.max(0, Math.min(deltaRaw, Math.max(0, H))); // clamp reasonably
            // ORIGINAL path shape; pick r & midX within available space
            const rHorizMax = Math.max(0, (H - delta) / 2);
            let r = Math.min(config.cornerRadius, rHorizMax);
            const midX = (startX + endX - direction * delta) / 2;
            let d;
            if (verticalOffset < 1e-6) {
                d = `M ${startX} ${y1} L ${endX} ${y2}`;
            }
            else {
                if (verticalOffset < 2 * r) {
                    const smallR = verticalOffset / 2;
                    const sweep1 = direction * ySign > 0 ? 1 : 0;
                    const sweep2 = direction * ySign > 0 ? 0 : 1;
                    d =
                        `M ${startX} ${y1}` +
                            ` L ${midX - smallR * direction} ${y1}` +
                            ` A ${smallR} ${smallR} 0 0 ${sweep1} ${midX} ${y1 + smallR * ySign}` +
                            ` A ${smallR} ${smallR} 0 0 ${sweep2} ${midX + smallR * direction} ${y2}` +
                            ` L ${endX} ${y2}`;
                }
                else {
                    const sweep1 = direction * ySign > 0 ? 1 : 0;
                    const sweep2 = direction * ySign > 0 ? 0 : 1;
                    d =
                        `M ${startX} ${y1}` +
                            ` L ${midX - r * direction} ${y1}` +
                            ` A ${r} ${r} 0 0 ${sweep1} ${midX} ${y1 + r * ySign}` +
                            ` L ${midX} ${y2 - r * ySign}` +
                            ` A ${r} ${r} 0 0 ${sweep2} ${midX + r * direction} ${y2}` +
                            ` L ${endX} ${y2}`;
                }
            }
            emitConnector(svg, d, parentData, childData, startX, y1, endX, y2, keyToPaletteMap, allKeys, containerId);
        }
        Renderer.createRoundedConnector = createRoundedConnector;
        function emitConnector(svg, d, parentData, childData, startX, y1, endX, y2, keyToPaletteMap, allKeys, containerId) {
            const parentKeys = parentData.id === 'main-anchor' ? allKeys : (parentData.sourceKeysSet || new Set());
            const childKeys = childData.sourceKeysSet || new Set();
            const commonKeys = [...childKeys].filter(key => parentKeys.has(key));
            const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
            group.dataset.sourceKeys = JSON.stringify(commonKeys);
            group.id = `group-conn-${containerId}-${childData.id}`;
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
                        gradient.innerHTML = createGradientStops(commonKeys, keyToPaletteMap, colorProp, 0.1);
                        return gradient;
                    };
                    if (!defs.querySelector(`#${baseGradientId}`)) {
                        defs.appendChild(createGradient(baseGradientId, 'hex'));
                    }
                    if (!defs.querySelector(`#${highlightGradientId}`)) {
                        defs.appendChild(createGradient(highlightGradientId, 'hexSat'));
                    }
                    basePath.style.stroke = `url(#${baseGradientId})`;
                    highlightPath.style.stroke = `url(#${highlightGradientId})`;
                }
            }
            group.appendChild(basePath);
            group.appendChild(highlightPath);
            svg.insertBefore(group, svg.firstChild);
        }
    })(Renderer = WordTree.Renderer || (WordTree.Renderer = {}));
})(WordTree || (WordTree = {}));
//# sourceMappingURL=word-tree-renderer.js.map