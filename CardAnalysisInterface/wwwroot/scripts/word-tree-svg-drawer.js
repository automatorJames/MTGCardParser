// word-tree-svg-drawer.ts
import { getFanDelta } from './word-tree-layout-calculator.js';
/**
 * Generates the SVG <stop> elements for a gradient based on a set of keys.
 * @param sourceKeys The keys to generate gradient stops for.
 * @param keyToPaletteMap A map from key to its color palette.
 * @param colorProperty The color property to use from the palette ('hex' or 'hexSat').
 * @param transitionRatio The ratio of each color band to use for smooth transitions.
 * @returns An HTML string of <stop> elements.
 */
export function createGradientStops(sourceKeys, keyToPaletteMap, colorProperty, transitionRatio) {
    const numKeys = sourceKeys.length;
    if (numKeys === 0)
        return '';
    if (numKeys === 1) {
        const palette = keyToPaletteMap.get(sourceKeys[0]);
        const color = palette ? palette[colorProperty] : '#ccc';
        return `<stop offset="0%" stop-color="${color}" /><stop offset="100%" stop-color="${color}" />`;
    }
    const clampedRatio = Math.max(0, Math.min(1, transitionRatio));
    const transitionZoneWidth = (1 / numKeys) * clampedRatio;
    const halfTransition = transitionZoneWidth / 2;
    let stopsHtml = '';
    sourceKeys.forEach((key, index) => {
        const palette = keyToPaletteMap.get(key);
        const color = palette ? palette[colorProperty] : '#ccc';
        const bandStart = index / numKeys;
        const bandEnd = (index + 1) / numKeys;
        const solidStartOffset = (index === 0) ? bandStart : bandStart + halfTransition;
        const solidEndOffset = (index === numKeys - 1) ? bandEnd : bandEnd - halfTransition;
        stopsHtml += `<stop offset="${solidStartOffset * 100}%" stop-color="${color}" />`;
        stopsHtml += `<stop offset="${solidEndOffset * 100}%" stop-color="${color}" />`;
    });
    return stopsHtml;
}
/**
 * Creates and appends a styled SVG group representing a single node.
 * @param svg The parent SVG element.
 * @param nodeData The data for the node to create.
 * @param centerX The x-coordinate of the node's center.
 * @param centerY The y-coordinate of the node's center.
 * @param isAdjacencyNode A flag indicating if this is a standard node (vs. the main anchor).
 * @param config The rendering configuration.
 * @param keyToPaletteMap A map from key to its color palette.
 * @param containerId The ID of the parent container, for creating unique gradient IDs.
 */
export function createNode(svg, nodeData, centerX, centerY, isAdjacencyNode, config, keyToPaletteMap, containerId) {
    const { dynamicHeight, wrappedLines, lineHeight } = nodeData;
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.setAttribute('class', 'node-group');
    group.id = isAdjacencyNode ? `group-node-${containerId}-${nodeData.id}` : `group-node-${containerId}-main-anchor`;
    const baseShape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    baseShape.setAttribute('class', 'node-shape base-layer');
    baseShape.setAttribute('x', `${-config.nodeWidth / 2}`);
    baseShape.setAttribute('y', `${-dynamicHeight / 2}`);
    baseShape.setAttribute('width', `${config.nodeWidth}`);
    baseShape.setAttribute('height', `${dynamicHeight}`);
    baseShape.setAttribute('rx', "8");
    const highlightShape = baseShape.cloneNode();
    highlightShape.classList.remove('base-layer');
    highlightShape.setAttribute('class', 'highlight-overlay');
    group.appendChild(baseShape);
    group.appendChild(highlightShape);
    if (isAdjacencyNode) {
        group.dataset.sourceKeys = JSON.stringify(nodeData.sourceOccurrenceKeys || []);
        const sourceKeys = nodeData.sourceOccurrenceKeys || [];
        if (sourceKeys.length > 0) {
            const defs = svg.querySelector('defs');
            if (defs) {
                const baseGradientId = `grad-node-base-${containerId}-${nodeData.id}`;
                const baseGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                baseGradient.setAttribute('id', baseGradientId);
                baseGradient.innerHTML = createGradientStops(sourceKeys, keyToPaletteMap, 'hex', config.gradientTransitionRatio);
                defs.appendChild(baseGradient);
                baseShape.style.stroke = `url(#${baseGradientId})`;
                const highlightGradientId = `grad-node-highlight-${containerId}-${nodeData.id}`;
                const highlightGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                highlightGradient.setAttribute('id', highlightGradientId);
                highlightGradient.innerHTML = createGradientStops(sourceKeys, keyToPaletteMap, 'hexSat', config.gradientTransitionRatio);
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
    const textElement = document.createElementNS("http://www.w3.org/2000/svg", "text");
    textElement.setAttribute('class', 'node-text');
    textElement.style.fontSize = `12px`;
    const totalTextHeight = wrappedLines.length * lineHeight;
    const startY = -totalTextHeight / 2 + lineHeight * 0.8;
    wrappedLines.forEach((line, index) => {
        const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tspan.setAttribute('x', '0');
        tspan.setAttribute('dy', index === 0 ? `${startY}` : `${lineHeight}`);
        tspan.textContent = line;
        textElement.appendChild(tspan);
    });
    group.appendChild(textElement);
    group.setAttribute('transform', `translate(${centerX}, ${centerY})`);
    svg.appendChild(group);
}
/**
 * Creates and appends a rounded SVG path to connect a parent and child node.
 * @param svg The parent SVG element.
 * @param parentData The data object for the parent node.
 * @param childData The data object for the child node.
 * @param direction The layout direction (-1 for left, 1 for right).
 * @param config The rendering configuration.
 * @param keyToPaletteMap A map from key to its color palette.
 * @param allKeys The complete set of keys for the entire tree.
 * @param containerId The ID of the parent container for unique gradient IDs.
 */
export function createRoundedConnector(svg, parentData, childData, direction, config, keyToPaletteMap, allKeys, containerId) {
    const [x1, y1] = [parentData.layout.x, parentData.layout.y];
    const [x2, y2] = [childData.layout.x, childData.layout.y];
    const startX = x1 + (direction * config.nodeWidth / 2);
    const endX = x2 - (direction * config.nodeWidth / 2);
    const verticalOffset = Math.abs(y2 - y1);
    const horizontalBudget = direction * (endX - startX); // Should be > 0
    // Adjust path based on pre-calculated fan-out delta.
    const fanDelta = Math.max(0, Math.min(getFanDelta(childData), Math.max(0, horizontalBudget)));
    const cornerRadiusMax = Math.max(0, (horizontalBudget - fanDelta) / 2);
    const cornerRadius = Math.min(config.cornerRadius, cornerRadiusMax);
    const midX = (startX + endX - direction * fanDelta) / 2;
    const ySign = Math.sign(y2 - y1) || 1;
    let pathData;
    if (verticalOffset < 1e-6) { // Straight horizontal line
        pathData = `M ${startX} ${y1} L ${endX} ${y2}`;
    }
    else {
        const effectiveRadius = Math.min(cornerRadius, verticalOffset / 2);
        const sweep1 = direction * ySign > 0 ? 1 : 0;
        const sweep2 = direction * ySign > 0 ? 0 : 1;
        pathData =
            `M ${startX} ${y1}` +
                ` L ${midX - effectiveRadius * direction} ${y1}` +
                ` A ${effectiveRadius} ${effectiveRadius} 0 0 ${sweep1} ${midX} ${y1 + effectiveRadius * ySign}` +
                ` L ${midX} ${y2 - effectiveRadius * ySign}` +
                ` A ${effectiveRadius} ${effectiveRadius} 0 0 ${sweep2} ${midX + effectiveRadius * direction} ${y2}` +
                ` L ${endX} ${y2}`;
    }
    const parentKeys = parentData.id === 'main-anchor' ? allKeys : (parentData.sourceKeysSet || new Set());
    const childKeys = childData.sourceKeysSet || new Set();
    const commonKeys = [...childKeys].filter(key => parentKeys.has(key));
    emitConnector(svg, pathData, childData, commonKeys, startX, y1, endX, y2, keyToPaletteMap, containerId);
}
/**
 * Low-level function to create the SVG connector elements (base and highlight paths).
 * @param svg The parent SVG element.
 * @param pathData The SVG path data string.
 * @param childData The data for the child node (for ID generation).
 * @param commonKeys The keys shared between the parent and child.
 * @param startX The starting X coordinate of the connector.
 * @param startY The starting Y coordinate of the connector.
 * @param endX The ending X coordinate of the connector.
 * @param endY The ending Y coordinate of the connector.
 * @param keyToPaletteMap A map from key to its color palette.
 * @param containerId The ID of the parent container for unique gradient IDs.
 */
function emitConnector(svg, pathData, childData, commonKeys, startX, startY, endX, endY, keyToPaletteMap, containerId) {
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.dataset.sourceKeys = JSON.stringify(commonKeys);
    group.id = `group-conn-${containerId}-${childData.id}`;
    const basePath = document.createElementNS("http://www.w3.org/2000/svg", "path");
    basePath.setAttribute('class', 'connector-path base-layer');
    basePath.setAttribute('d', pathData);
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
                gradient.setAttribute('x1', `${startX}`);
                gradient.setAttribute('y1', `${startY}`);
                gradient.setAttribute('x2', `${endX}`);
                gradient.setAttribute('y2', `${endY}`);
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
    svg.insertBefore(group, svg.firstChild); // Insert connectors behind nodes
}
/**
 * Recursively draws all nodes and their connectors for a given tree.
 * @param svg The parent SVG element.
 * @param nodes The array of nodes to draw.
 * @param parentData The data of the parent node.
 * @param direction The layout direction (-1 for preceding, 1 for following).
 * @param config The rendering configuration.
 * @param keyToPaletteMap A map from key to its color palette.
 * @param allKeys The complete set of keys for the entire tree.
 * @param containerId The ID of the parent container for unique IDs.
 */
export function drawNodesAndConnectors(svg, nodes, parentData, direction, config, keyToPaletteMap, allKeys, containerId) {
    if (!nodes)
        return;
    for (const node of nodes) {
        createRoundedConnector(svg, parentData, node, direction, config, keyToPaletteMap, allKeys, containerId);
        createNode(svg, node, node.layout.x, node.layout.y, true, config, keyToPaletteMap, containerId);
        if (node.children) {
            drawNodesAndConnectors(svg, node.children, node, direction, config, keyToPaletteMap, allKeys, containerId);
        }
    }
}
//# sourceMappingURL=word-tree-svg-drawer.js.map