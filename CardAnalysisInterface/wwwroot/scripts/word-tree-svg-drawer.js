// word-tree-svg-drawer.ts
import { getFanDelta } from './word-tree-layout-calculator.js';
/**
 * Generates the SVG <stop> elements for a gradient.
 */
export function createGradientStops(sourceCardNames, paletteMap, colorProperty, transitionRatio) {
    const numKeys = sourceCardNames.length;
    if (numKeys === 0)
        return '';
    if (numKeys === 1) {
        const color = paletteMap.get(sourceCardNames[0])?.[colorProperty] ?? '#ccc';
        return `<stop offset="0%" stop-color="${color}" /><stop offset="100%" stop-color="${color}" />`;
    }
    const clampedRatio = Math.max(0, Math.min(1, transitionRatio));
    const step = 1 / numKeys;
    const halfTransition = (step * clampedRatio) / 2;
    let stopsHtml = '';
    sourceCardNames.forEach((key, index) => {
        const color = paletteMap.get(key)?.[colorProperty] ?? '#ccc';
        const bandStart = index * step;
        const bandEnd = bandStart + step;
        const solidStartOffset = (index === 0) ? bandStart : bandStart + halfTransition;
        const solidEndOffset = (index === numKeys - 1) ? bandEnd : bandEnd - halfTransition;
        stopsHtml += `<stop offset="${solidStartOffset * 100}%" stop-color="${color}" />`;
        stopsHtml += `<stop offset="${solidEndOffset * 100}%" stop-color="${color}" />`;
    });
    return stopsHtml;
}
/**
 * Creates and appends a styled SVG group representing a single node.
 */
export function createNode(svg, nodeData, isAdjacencyNode, config, paletteMap, containerId) {
    const { dynamicHeight, wrappedLines, lineHeight, layout } = nodeData;
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.setAttribute('class', 'node-group');
    group.id = isAdjacencyNode ? `group-node-${containerId}-${nodeData.id}` : `group-node-${containerId}-main-anchor`;
    const baseShape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    baseShape.setAttribute('class', 'node-shape base-layer');
    baseShape.setAttribute('x', `${-config.nodeWidth / 2}`);
    baseShape.setAttribute('y', `${-dynamicHeight / 2}`);
    baseShape.setAttribute('width', `${config.nodeWidth}`);
    baseShape.setAttribute('height', `${dynamicHeight}`);
    baseShape.setAttribute('rx', `${config.cornerRadius}`);
    const highlightShape = baseShape.cloneNode();
    highlightShape.classList.remove('base-layer');
    highlightShape.setAttribute('class', 'highlight-overlay');
    group.append(baseShape, highlightShape);
    if (isAdjacencyNode) {
        group.dataset.sourceKeys = JSON.stringify(nodeData.sourceOccurrenceKeys || []);
        const sourceCardNames = nodeData.sourceOccurrenceKeys || [];
        if (sourceCardNames.length > 0) {
            const defs = svg.querySelector('defs');
            if (defs) {
                // Define and apply base gradient for the border
                const baseGradientId = `grad-node-base-${containerId}-${nodeData.id}`;
                const baseGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                baseGradient.id = baseGradientId;
                baseGradient.innerHTML = createGradientStops(sourceCardNames, paletteMap, 'hex', config.gradientTransitionRatio);
                defs.appendChild(baseGradient);
                baseShape.style.stroke = `url(#${baseGradientId})`;
                // Define and apply highlight gradient for the border
                const highlightGradientId = `grad-node-highlight-${containerId}-${nodeData.id}`;
                const highlightGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                highlightGradient.id = highlightGradientId;
                highlightGradient.innerHTML = createGradientStops(sourceCardNames, paletteMap, 'hexSat', config.gradientTransitionRatio);
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
    const totalTextHeight = wrappedLines.length * lineHeight;
    const startY = -totalTextHeight / 2 + lineHeight * 0.8; // Vertical centering adjustment
    wrappedLines.forEach((line, index) => {
        const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tspan.setAttribute('x', '0');
        tspan.setAttribute('dy', index === 0 ? `${startY}` : `${lineHeight}`);
        tspan.textContent = line;
        textElement.appendChild(tspan);
    });
    group.appendChild(textElement);
    group.setAttribute('transform', `translate(${layout.x}, ${layout.y})`);
    svg.appendChild(group);
}
/**
 * Creates and appends a rounded SVG path to connect a parent and child node.
 * This version uses a simplified "takeoff" logic for fanning.
 */
export function createRoundedConnector(svg, parentData, childData, direction, config, paletteMap, allCards, containerId) {
    const { x: x1, y: y1 } = parentData.layout;
    const { x: x2, y: y2 } = childData.layout;
    const startX = x1 + (direction * config.nodeWidth / 2);
    const endX = x2 - (direction * config.nodeWidth / 2);
    const fanDelta = getFanDelta(childData);
    const takeoffX = startX + (direction * fanDelta); // Point where the curve begins
    const verticalOffset = Math.abs(y2 - y1);
    const horizontalTurnDistance = Math.abs(endX - takeoffX);
    // Radius cannot exceed half the available horizontal or vertical space for the turn.
    const radius = Math.min(config.cornerRadius, horizontalTurnDistance / 2, verticalOffset / 2);
    const ySign = Math.sign(y2 - y1) || 1;
    let pathData;
    if (verticalOffset < 1e-6) { // Straight horizontal line (should be rare with fanning).
        pathData = `M ${startX} ${y1} L ${endX} ${y2}`;
    }
    else {
        const midTurnX = (takeoffX + endX) / 2;
        const sweep1 = direction * ySign > 0 ? 1 : 0;
        const sweep2 = direction * ySign > 0 ? 0 : 1;
        pathData =
            `M ${startX} ${y1}` + // Start at parent edge
                ` L ${takeoffX} ${y1}` + // Initial straight segment for fanning
                ` L ${midTurnX - radius * direction} ${y1}` + // Straight segment into the turn
                ` A ${radius} ${radius} 0 0 ${sweep1} ${midTurnX} ${y1 + radius * ySign}` + // First curve
                ` L ${midTurnX} ${y2 - radius * ySign}` + // Vertical segment
                ` A ${radius} ${radius} 0 0 ${sweep2} ${midTurnX + radius * direction} ${y2}` + // Second curve
                ` L ${endX} ${y2}`; // Final straight segment to child
    }
    const parentKeys = parentData.id === 'main-anchor' ? allCards : (parentData.sourceKeysSet || new Set());
    const childKeys = childData.sourceKeysSet || new Set();
    const commonKeys = [...childKeys].filter(key => parentKeys.has(key));
    emitConnector(svg, pathData, childData, commonKeys, startX, y1, endX, y2, paletteMap, containerId);
}
/**
 * Low-level function to create the SVG connector elements (base and highlight paths).
 */
function emitConnector(svg, pathData, childData, commonCardNames, startX, startY, endX, endY, paletteMap, containerId) {
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.dataset.sourceKeys = JSON.stringify(commonCardNames);
    group.id = `group-conn-${containerId}-${childData.id}`;
    const basePath = document.createElementNS("http://www.w3.org/2000/svg", "path");
    basePath.setAttribute('class', 'connector-path base-layer');
    basePath.setAttribute('d', pathData);
    const highlightPath = basePath.cloneNode();
    highlightPath.classList.remove('base-layer');
    highlightPath.setAttribute('class', 'highlight-overlay');
    if (commonCardNames.length > 0) {
        const defs = svg.querySelector('defs');
        if (defs) {
            const idSuffix = `${containerId}-${childData.id}`;
            const baseGradientId = `grad-conn-base-${idSuffix}`;
            const highlightGradientId = `grad-conn-highlight-${idSuffix}`;
            const createGradient = (id, colorProp) => {
                const gradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                gradient.id = id;
                gradient.setAttribute('gradientUnits', 'userSpaceOnUse');
                gradient.setAttribute('x1', `${startX}`);
                gradient.setAttribute('y1', `${startY}`);
                gradient.setAttribute('x2', `${endX}`);
                gradient.setAttribute('y2', `${endY}`);
                gradient.innerHTML = createGradientStops(commonCardNames, paletteMap, colorProp, 0.1);
                return gradient;
            };
            // Avoid creating duplicate gradients
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
    group.append(basePath, highlightPath);
    svg.insertBefore(group, svg.firstChild); // Insert connectors behind nodes
}
/**
 * Recursively draws all nodes and their connectors for a given tree.
 */
export function drawNodesAndConnectors(svg, nodes, parentData, direction, config, paletteMap, allCards, containerId) {
    if (!nodes)
        return;
    for (const node of nodes) {
        // Draw connector from parent to this node first (so it's in the background)
        createRoundedConnector(svg, parentData, node, direction, config, paletteMap, allCards, containerId);
        // Draw the node itself
        createNode(svg, node, true, config, paletteMap, containerId);
        // Recurse for children
        if (node.children) {
            drawNodesAndConnectors(svg, node.children, node, direction, config, paletteMap, allCards, containerId);
        }
    }
}
//# sourceMappingURL=word-tree-svg-drawer.js.map