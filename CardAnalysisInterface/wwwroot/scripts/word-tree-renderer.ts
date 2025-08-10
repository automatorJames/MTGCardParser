import { AdjacencyNode, DeterministicPalette } from './models.js';

// This file contains all the logic for calculating layout and drawing the SVG.
// It is self-contained and does not handle events or animation.

export namespace WordTree.Renderer {

    export interface NodeConfig {
        nodeWidth: number;
        nodePadding: number;
        mainSpanPadding: number;
        nodeHeight: number;
        hGap: number;
        vGap: number;
        cornerRadius: number;
        mainSpanWidth: number;
        mainSpanFontSize: number;
        mainSpanLineHeight: number;
        mainSpanFill: string;
        mainSpanColor: string;
        horizontalPadding: number;
        gradientTransitionRatio: number;
    }

    /**
     * Generates SVG <stop> elements for a gradient.
     * @private
     */
    function _createGradientStops(
        keys: string[],
        keyToPaletteMap: Map<string, DeterministicPalette>,
        colorProperty: 'hex' | 'hexLight',
        transitionRatio: number
    ): string {
        const numKeys = keys.length;
        if (numKeys === 0) return '';

        // Fallback for a simple case of a single color.
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

    export function preCalculateAllNodeMetrics(node: any, isAnchor: boolean, config: NodeConfig, svg: SVGSVGElement): void {
        if (!node) return;
        const metrics = getNodeMetrics(node.text, isAnchor, config, svg);
        node.dynamicHeight = metrics.dynamicHeight;
        node.wrappedLines = metrics.wrappedLines;
        node.lineHeight = metrics.lineHeight;
        if (node.children) node.children.forEach((child: AdjacencyNode) => preCalculateAllNodeMetrics(child, false, config, svg));
    }

    export function getNodeMetrics(text: string, isAnchor: boolean, config: NodeConfig, svg: SVGSVGElement): { dynamicHeight: number, wrappedLines: string[], lineHeight: number } {
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
        const wrappedLines: string[] = [];
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

    export function calculateLayout(nodes: AdjacencyNode[], depth: number, parentX: number, parentY: number, direction: number, config: NodeConfig): { layout: AdjacencyNode[], totalHeight: number } {
        if (!nodes || nodes.length === 0) return { layout: [], totalHeight: 0 };
        const layoutInfo: AdjacencyNode[] = [];
        const nodeMetrics: { node: AdjacencyNode, effectiveHeight: number }[] = [];
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

    export function drawNodesAndConnectors(svg: SVGSVGElement, nodes: AdjacencyNode[], parentData: any, parentX: number, parentY: number, direction: number, config: NodeConfig, keyToPaletteMap: Map<string, DeterministicPalette>, allKeys: Set<string>, containerId: string): void {
        if (!nodes) return;
        for (const node of nodes) {
            const { x: nodeX, y: nodeY } = node.layout;
            createRoundedConnector(svg, parentData, node, parentX, parentY, nodeX, nodeY, direction, config, keyToPaletteMap, allKeys, containerId);
            createNode(svg, node, nodeX, nodeY, true, config, keyToPaletteMap, containerId);
            if (node.children) {
                drawNodesAndConnectors(svg, node.children, node, nodeX, nodeY, direction, config, keyToPaletteMap, allKeys, containerId);
            }
        }
    }

    export function createNode(svg: SVGSVGElement, nodeData: any, cx: number, cy: number, isAdjacencyNode: boolean, config: NodeConfig, keyToPaletteMap: Map<string, DeterministicPalette>, containerId: string): void {
        const { dynamicHeight, wrappedLines, lineHeight } = nodeData;
        const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
        group.setAttribute('class', 'node-group');

        const nodeWidth = isAdjacencyNode ? config.nodeWidth : config.mainSpanWidth;
        const baseShape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
        baseShape.setAttribute('class', 'node-shape base-layer');
        baseShape.setAttribute('x', `${-nodeWidth / 2}`); baseShape.setAttribute('y', `${-dynamicHeight / 2}`);
        baseShape.setAttribute('width', `${nodeWidth}`); baseShape.setAttribute('height', `${dynamicHeight}`); baseShape.setAttribute('rx', "8");

        const highlightShape = baseShape.cloneNode() as SVGRectElement;
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
                    // Create gradient for the base color
                    const baseGradientId = `grad-node-base-${containerId}-${nodeData.id}`;
                    const baseGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                    baseGradient.setAttribute('id', baseGradientId);
                    baseGradient.innerHTML = _createGradientStops(keys, keyToPaletteMap, 'hex', config.gradientTransitionRatio);
                    defs.appendChild(baseGradient);
                    baseShape.style.stroke = `url(#${baseGradientId})`;

                    // Create gradient for the highlight color
                    const highlightGradientId = `grad-node-highlight-${containerId}-${nodeData.id}`;
                    const highlightGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                    highlightGradient.setAttribute('id', highlightGradientId);
                    highlightGradient.innerHTML = _createGradientStops(keys, keyToPaletteMap, 'hexLight', config.gradientTransitionRatio);
                    defs.appendChild(highlightGradient);
                    highlightShape.style.stroke = `url(#${highlightGradientId})`;
                }
            }
        } else {
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
        wrappedLines.forEach((line: string, i: number) => {
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

    export function createRoundedConnector(svg: SVGSVGElement, parentData: any, childData: AdjacencyNode, x1: number, y1: number, x2: number, y2: number, direction: number, config: NodeConfig, keyToPaletteMap: Map<string, DeterministicPalette>, allKeys: Set<string>, containerId: string): void {
        const parentWidth = parentData.id === 'main-anchor' ? config.mainSpanWidth : config.nodeWidth;
        const startX = x1 + (direction * parentWidth / 2);
        const endX = x2 - (direction * config.nodeWidth / 2);
        const midX = (startX + endX) / 2;
        const r = config.cornerRadius;
        const verticalOffset = Math.abs(y2 - y1);
        let d: string;

        // FIX 1: Explicitly handle perfectly horizontal lines.
        if (verticalOffset < 1e-6) {
            d = `M ${startX} ${y1} L ${endX} ${y2}`;
        } else if (verticalOffset < r * 2) {
            // Case for shallow curves where full radius isn't possible.
            const smallR = verticalOffset / 2;
            const ySign = Math.sign(y2 - y1);
            const sweepFlag1 = direction * ySign > 0 ? 1 : 0;
            const sweepFlag2 = direction * ySign > 0 ? 0 : 1;
            d = `M ${startX} ${y1} L ${midX - smallR * direction} ${y1} A ${smallR} ${smallR} 0 0 ${sweepFlag1} ${midX} ${y1 + smallR * ySign} A ${smallR} ${smallR} 0 0 ${sweepFlag2} ${midX + smallR * direction} ${y2} L ${endX} ${y2}`;
        } else {
            // Standard case for curved connectors.
            const ySign = Math.sign(y2 - y1);
            const sweepFlag1 = direction * ySign > 0 ? 1 : 0;
            const sweepFlag2 = direction * ySign > 0 ? 0 : 1;
            d = `M ${startX} ${y1} L ${midX - r * direction} ${y1} A ${r} ${r} 0 0 ${sweepFlag1} ${midX} ${y1 + r * ySign} L ${midX} ${y2 - r * ySign} A ${r} ${r} 0 0 ${sweepFlag2} ${midX + r * direction} ${y2} L ${endX} ${y2}`;
        }

        const parentKeys = parentData.id === 'main-anchor' ? allKeys : new Set<string>(parentData.sourceOccurrenceKeys || []);
        const childKeys = new Set<string>(childData.sourceOccurrenceKeys || []);
        const commonKeys = [...childKeys].filter(key => parentKeys.has(key));

        const connectorGroup = document.createElementNS("http://www.w3.org/2000/svg", "g");
        connectorGroup.dataset.sourceKeys = JSON.stringify(commonKeys);

        const basePath = document.createElementNS("http://www.w3.org/2000/svg", "path");
        basePath.setAttribute('class', 'connector-path base-layer');
        basePath.setAttribute('d', d);

        const highlightPath = basePath.cloneNode() as SVGPathElement;
        highlightPath.classList.remove('base-layer');
        highlightPath.setAttribute('class', 'highlight-overlay');

        if (commonKeys.length > 0) {
            const defs = svg.querySelector('defs');
            if (defs) {
                const idSuffix = `${containerId}-${childData.id}`;
                const baseGradientId = `grad-conn-base-${idSuffix}`;
                const highlightGradientId = `grad-conn-highlight-${idSuffix}`;

                // FIX 2: Correctly define the gradient with proper units and orientation.
                const createGradient = (id: string, colorProp: 'hex' | 'hexLight') => {
                    const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                    gradient.setAttribute('id', id);
                    gradient.setAttribute('gradientUnits', 'userSpaceOnUse');

                    const deltaX = endX - startX;
                    const deltaY = y2 - y1;

                    // Set gradient vector to match the dominant direction of the line
                    if (Math.abs(deltaY) > Math.abs(deltaX)) {
                        gradient.setAttribute('x1', '0'); gradient.setAttribute('y1', `${y1}`);
                        gradient.setAttribute('x2', '0'); gradient.setAttribute('y2', `${y2}`);
                    } else {
                        gradient.setAttribute('x1', `${startX}`); gradient.setAttribute('y1', '0');
                        gradient.setAttribute('x2', `${endX}`); gradient.setAttribute('y2', '0');
                    }

                    // Keys must be reversed to match original JS logic for gradient direction
                    const reversedKeys = [...commonKeys].reverse();
                    gradient.innerHTML = _createGradientStops(reversedKeys, keyToPaletteMap, colorProp, config.gradientTransitionRatio);
                    return gradient;
                };

                if (!defs.querySelector(`#${baseGradientId}`)) {
                    defs.appendChild(createGradient(baseGradientId, 'hex'));
                }
                if (!defs.querySelector(`#${highlightGradientId}`)) {
                    defs.appendChild(createGradient(highlightGradientId, 'hexLight'));
                }

                basePath.style.stroke = `url(#${baseGradientId})`;
                highlightPath.style.stroke = `url(#${highlightGradientId})`;
            }
        }

        connectorGroup.appendChild(basePath);
        connectorGroup.appendChild(highlightPath);
        svg.insertBefore(connectorGroup, svg.firstChild);
    }
}