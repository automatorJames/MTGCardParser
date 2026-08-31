import { getFanDelta } from './echo-tree-layout-calculator.js';
const SVG_NS = "http://www.w3.org/2000/svg";
const svgElement = (name) => document.createElementNS(SVG_NS, name);
/**
 * Generates the SVG <stop> elements for a gradient: one equal-width band per source document, so
 * a node shared by three documents reads as three colors around its border.
 */
export function createGradientStops(sourceDocumentNames, paletteMap, variant, transitionRatio) {
    const numKeys = sourceDocumentNames.length;
    if (numKeys === 0)
        return '';
    if (numKeys === 1) {
        const color = paletteMap.get(sourceDocumentNames[0])?.[variant] ?? '#ccc';
        return `<stop offset="0%" stop-color="${color}" /><stop offset="100%" stop-color="${color}" />`;
    }
    const clampedRatio = Math.max(0, Math.min(1, transitionRatio));
    const step = 1 / numKeys;
    const halfTransition = (step * clampedRatio) / 2;
    let stopsHtml = '';
    sourceDocumentNames.forEach((key, index) => {
        const color = paletteMap.get(key)?.[variant] ?? '#ccc';
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
 * Tags a group with the documents it belongs to and builds its two stroke gradients: the resting
 * one painted straight onto the base layer, and the saturated one on the overlay that hover fades
 * in. Both the key set and the highlight gradient are attached to the element itself so the hover
 * handler can reach them without parsing attributes or reconstructing ids.
 */
function attachSourceKeys(ctx, group, sourceKeys, baseTarget, highlightTarget, gradientId, configureGradient) {
    const keyed = group;
    keyed.classList.add('wt-keyed');
    keyed.__sourceKeys = sourceKeys;
    keyed.__sourceKeysSet = new Set(sourceKeys);
    keyed.__highlightGradient = null;
    if (sourceKeys.length === 0)
        return keyed;
    const buildGradient = (id, variant) => {
        const gradient = svgElement('linearGradient');
        gradient.id = id;
        configureGradient?.(gradient);
        gradient.innerHTML = createGradientStops(sourceKeys, ctx.data.documentPalettes, variant, ctx.config.gradientTransitionRatio);
        ctx.defs.appendChild(gradient);
        return gradient;
    };
    const baseGradient = buildGradient(`grad-${gradientId}-base`, 'normal');
    const highlightGradient = buildGradient(`grad-${gradientId}-highlight`, 'sat');
    baseTarget.style.stroke = `url(#${baseGradient.id})`;
    highlightTarget.style.stroke = `url(#${highlightGradient.id})`;
    keyed.__highlightGradient = highlightGradient;
    return keyed;
}
/**
 * Clones the two stroke overlays that stack on top of every node's and connector's resting layer:
 * the saturated highlight that fades in for the hovered documents, and the white one used for
 * anchor hover. Both are geometrically identical to the base layer, so they're cloned from it.
 */
function createOverlayLayers(baseLayer) {
    const highlight = baseLayer.cloneNode();
    highlight.setAttribute('class', 'highlight-overlay');
    const anchorHover = baseLayer.cloneNode();
    anchorHover.setAttribute('class', 'anchor-hover-overlay');
    return [highlight, anchorHover];
}
/**
 * Renders a node's text as one tspan per wrapped line, sub-split into one tspan per glyph-typed
 * run. Captured runs carry their Glyph type name plus that type's palette as CSS variables, which
 * is what lets stylesheet rules - not inline fills - drive their hover treatment.
 */
function createNodeText(ctx, nodeData) {
    const textElement = svgElement('text');
    textElement.setAttribute('class', 'node-text');
    const { wrappedLines, lineHeight } = nodeData;
    const startY = -(wrappedLines.length * lineHeight) / 2 + lineHeight * 0.8;
    wrappedLines.forEach((line, lineIndex) => {
        const lineTspan = svgElement('tspan');
        lineTspan.setAttribute('x', '0');
        lineTspan.setAttribute('dy', lineIndex === 0 ? `${startY}` : `${lineHeight}`);
        for (const chunk of line.chunks) {
            const chunkTspan = svgElement('tspan');
            chunkTspan.textContent = chunk.text;
            chunkTspan.classList.add('node-text-content');
            const palette = chunk.glyphType ? ctx.data.glyphPalettes.get(chunk.glyphType) : undefined;
            if (chunk.glyphType && palette) {
                chunkTspan.classList.add('interactive-subspan');
                chunkTspan.dataset.glyphType = chunk.glyphType;
                chunkTspan.style.setProperty('--glyph-color', palette.light);
                chunkTspan.style.setProperty('--glyph-sat-color', palette.sat);
            }
            lineTspan.appendChild(chunkTspan);
        }
        textElement.appendChild(lineTspan);
    });
    return textElement;
}
/**
 * Creates and appends a styled SVG group representing a single node.
 * @param column `${direction}:${depth}` for an adjacency node, or null for the central anchor.
 */
export function createNode(ctx, nodeData, column) {
    const { config, containerId } = ctx;
    const { dynamicHeight, layout } = nodeData;
    const isAnchor = column === null;
    const group = svgElement('g');
    group.setAttribute('class', 'node-group');
    group.id = `group-node-${containerId}-${nodeData.id}`;
    const baseShape = svgElement('rect');
    baseShape.setAttribute('class', 'node-shape base-layer');
    baseShape.setAttribute('x', `${-config.nodeWidth / 2}`);
    baseShape.setAttribute('y', `${-dynamicHeight / 2}`);
    baseShape.setAttribute('width', `${config.nodeWidth}`);
    baseShape.setAttribute('height', `${dynamicHeight}`);
    baseShape.setAttribute('rx', `${config.cornerRadius}`);
    const [highlightShape, anchorHoverShape] = createOverlayLayers(baseShape);
    group.append(baseShape, highlightShape, anchorHoverShape);
    if (isAnchor) {
        group.classList.add('main-anchor-span');
        baseShape.style.setProperty('--node-border-color', config.mainSpanColor);
    }
    else {
        const keyed = attachSourceKeys(ctx, group, nodeData.sourceOccurrenceDocumentNames || [], baseShape, highlightShape, `node-${containerId}-${nodeData.id}`);
        keyed.__column = column;
    }
    group.appendChild(createNodeText(ctx, nodeData));
    group.setAttribute('transform', `translate(${layout.x}, ${layout.y})`);
    ctx.svg.appendChild(group);
}
/**
 * Creates and appends a rounded SVG path connecting a parent and child node, colored by the
 * documents the two have in common.
 */
function createRoundedConnector(ctx, parentData, childData, direction) {
    const { config, containerId } = ctx;
    const { x: x1, y: y1 } = parentData.layout;
    const { x: x2, y: y2 } = childData.layout;
    const startX = x1 + (direction * config.nodeWidth / 2);
    const endX = x2 - (direction * config.nodeWidth / 2);
    const fanDelta = getFanDelta(childData);
    const takeoffX = startX + (direction * fanDelta);
    const verticalOffset = Math.abs(y2 - y1);
    const horizontalTurnDistance = Math.abs(endX - takeoffX);
    const radius = Math.min(config.cornerRadius, horizontalTurnDistance / 2, verticalOffset / 2);
    const ySign = Math.sign(y2 - y1) || 1;
    let pathData;
    if (verticalOffset < 1e-6) {
        pathData = `M ${startX} ${y1} L ${endX} ${y2}`;
    }
    else {
        const midTurnX = (takeoffX + endX) / 2;
        const sweep1 = direction * ySign > 0 ? 1 : 0;
        const sweep2 = direction * ySign > 0 ? 0 : 1;
        pathData =
            `M ${startX} ${y1}` +
                ` L ${takeoffX} ${y1}` +
                ` L ${midTurnX - radius * direction} ${y1}` +
                ` A ${radius} ${radius} 0 0 ${sweep1} ${midTurnX} ${y1 + radius * ySign}` +
                ` L ${midTurnX} ${y2 - radius * ySign}` +
                ` A ${radius} ${radius} 0 0 ${sweep2} ${midTurnX + radius * direction} ${y2}` +
                ` L ${endX} ${y2}`;
    }
    const parentKeys = parentData.id === 'main-anchor'
        ? ctx.data.allDocumentsSet
        : (parentData.sourceKeysSet ?? new Set());
    const childKeys = childData.sourceKeysSet ?? new Set();
    const commonKeys = [...childKeys].filter(key => parentKeys.has(key));
    const group = svgElement('g');
    group.id = `group-conn-${containerId}-${childData.id}`;
    const basePath = svgElement('path');
    basePath.setAttribute('class', 'connector-path base-layer');
    basePath.setAttribute('d', pathData);
    const [highlightPath, anchorHoverPath] = createOverlayLayers(basePath);
    attachSourceKeys(ctx, group, commonKeys, basePath, highlightPath, `conn-${containerId}-${childData.id}`, gradient => {
        gradient.setAttribute('gradientUnits', 'userSpaceOnUse');
        gradient.setAttribute('x1', `${startX}`);
        gradient.setAttribute('y1', `${y1}`);
        gradient.setAttribute('x2', `${endX}`);
        gradient.setAttribute('y2', `${y2}`);
    });
    group.append(basePath, highlightPath, anchorHoverPath);
    // Connectors go behind every node so a path never crosses over a node's fill.
    ctx.svg.insertBefore(group, ctx.svg.firstChild);
}
/**
 * Recursively draws all nodes and their connectors for one side of the tree.
 * @param direction -1 for the preceding (leftward) tree, +1 for the following (rightward) tree.
 */
export function drawNodesAndConnectors(ctx, nodes, parentData, direction, depth = 1) {
    if (!nodes)
        return;
    for (const node of nodes) {
        createRoundedConnector(ctx, parentData, node, direction);
        createNode(ctx, node, `${direction}:${depth}`);
        drawNodesAndConnectors(ctx, node.children, node, direction, depth + 1);
    }
}
/**
 * Draws the two participation-count labels that fade in above and below the anchor on node hover
 * (how many documents pass through the hovered node, and - when they don't map one-to-one - how
 * many visually distinct throughlines those documents form). Empty and invisible until the hover
 * handler fills them in.
 */
export function createAnchorStatLabels(ctx, anchor) {
    const { config } = ctx;
    for (const placement of ['above', 'below']) {
        const label = svgElement('text');
        label.setAttribute('class', `tree-stat tree-stat-${placement}`);
        label.setAttribute('x', `${anchor.layout.x}`);
        label.setAttribute('text-anchor', 'middle');
        const edge = anchor.dynamicHeight / 2 + config.statLabelGap;
        label.setAttribute('y', `${anchor.layout.y + (placement === 'above' ? -edge : edge)}`);
        label.setAttribute('dominant-baseline', placement === 'above' ? 'auto' : 'hanging');
        ctx.svg.appendChild(label);
    }
}
//# sourceMappingURL=echo-tree-svg-drawer.js.map