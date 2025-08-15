import { AdjacencyNode, DeterministicPalette, NodeConfig } from './models.js';
import { getFanDelta } from './word-tree-layout-calculator.js';

/**
 * Generates the SVG <stop> elements for a gradient.
 */
export function createGradientStops(sourceCardNames: string[], paletteMap: Map<string, DeterministicPalette>, colorProperty: 'hex' | 'hexSat', transitionRatio: number): string {
    const numKeys = sourceCardNames.length;
    if (numKeys === 0) return '';
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
export function createNode(svg: SVGSVGElement, nodeData: any, isAdjacencyNode: boolean, config: NodeConfig, paletteMap: Map<string, DeterministicPalette>, containerId: string): void {
    const { dynamicHeight, layout } = nodeData;
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.setAttribute('class', 'node-group');
    group.id = isAdjacencyNode ? `group-node-${containerId}-${nodeData.id}` : `group-node-${containerId}-main-anchor`;
    if (isAdjacencyNode) {
        group.dataset.sourceKeys = JSON.stringify(nodeData.sourceOccurrenceKeys || []);
    }

    const baseShape = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    baseShape.setAttribute('class', 'node-shape base-layer');
    baseShape.setAttribute('x', `${-config.nodeWidth / 2}`);
    baseShape.setAttribute('y', `${-dynamicHeight / 2}`);
    baseShape.setAttribute('width', `${config.nodeWidth}`);
    baseShape.setAttribute('height', `${dynamicHeight}`);
    baseShape.setAttribute('rx', `${config.cornerRadius}`);

    const highlightShape = baseShape.cloneNode() as SVGRectElement;
    highlightShape.classList.remove('base-layer');
    highlightShape.setAttribute('class', 'highlight-overlay');
    group.append(baseShape, highlightShape);

    if (isAdjacencyNode) {
        const sourceCardNames = nodeData.sourceOccurrenceKeys || [];
        if (sourceCardNames.length > 0) {
            const defs = svg.querySelector('defs');
            if (defs) {
                const baseGradientId = `grad-node-base-${containerId}-${nodeData.id}`;
                const baseGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                baseGradient.id = baseGradientId;
                baseGradient.innerHTML = createGradientStops(sourceCardNames, paletteMap, 'hex', config.gradientTransitionRatio);
                defs.appendChild(baseGradient);
                baseShape.style.stroke = `url(#${baseGradientId})`;

                const highlightGradientId = `grad-node-highlight-${containerId}-${nodeData.id}`;
                const highlightGradient = document.createElementNS("http://www.w3.org/2000/svg", "linearGradient");
                highlightGradient.id = highlightGradientId;
                highlightGradient.innerHTML = createGradientStops(sourceCardNames, paletteMap, 'hexSat', config.gradientTransitionRatio);
                defs.appendChild(highlightGradient);
                highlightShape.style.stroke = `url(#${highlightGradientId})`;
            }
        }
    } else {
        group.classList.add('main-anchor-span');
        baseShape.style.fill = config.mainSpanFill;
        baseShape.style.setProperty('--node-border-color', config.mainSpanColor);
    }

    const textElement = document.createElementNS("http://www.w3.org/2000/svg", "text");
    textElement.setAttribute('class', 'node-text');

    const { text, spanPalettes, wrappedLines, lineHeight } = nodeData;
    const totalTextHeight = wrappedLines.length * lineHeight;
    const startY = -totalTextHeight / 2 + lineHeight * 0.8;

    const paletteEntries = spanPalettes ? Object.entries(spanPalettes) : [];

    if (paletteEntries.length === 0) {
        wrappedLines.forEach((line: string, index: number) => {
            const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
            tspan.setAttribute('x', '0');
            tspan.setAttribute('dy', index === 0 ? `${startY}` : `${lineHeight}`);
            tspan.textContent = line;
            tspan.classList.add('node-text-content');
            textElement.appendChild(tspan);
        });
    } else {
        const colorStops = paletteEntries
            .map(([index, palette]) => ({ index: parseInt(index, 10), palette: palette as DeterministicPalette }))
            .sort((a, b) => a.index - b.index);

        let charCursor = 0;
        wrappedLines.forEach((lineText: string, lineIndex: number) => {
            const lineTspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
            lineTspan.setAttribute('x', '0');
            lineTspan.setAttribute('dy', lineIndex === 0 ? `${startY}` : `${lineHeight}`);

            const lineStartIndex = text.indexOf(lineText, charCursor);
            let lineCursor = 0;

            while (lineCursor < lineText.length) {
                const absoluteCursor = lineStartIndex + lineCursor;
                let activePalette: DeterministicPalette | null = null;
                for (const stop of colorStops) {
                    if (stop.index <= absoluteCursor) activePalette = stop.palette;
                    else break;
                }

                let nextStopAbsoluteIndex = lineStartIndex + lineText.length;
                for (const stop of colorStops) {
                    if (stop.index > absoluteCursor) {
                        nextStopAbsoluteIndex = stop.index;
                        break;
                    }
                }

                const chunkEndIndexInLine = Math.min(lineText.length, nextStopAbsoluteIndex - lineStartIndex);
                const chunkText = lineText.substring(lineCursor, chunkEndIndexInLine);

                const subTspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
                subTspan.textContent = chunkText;
                subTspan.classList.add('node-text-content');

                if (activePalette) {
                    subTspan.style.fill = activePalette.hexLight;
                    subTspan.style.fontWeight = 'bold';
                    subTspan.classList.add('interactive-subspan');
                    if (activePalette.seed) subTspan.dataset.typeSeed = activePalette.seed;
                    subTspan.dataset.baseColor = activePalette.hexLight;
                    subTspan.dataset.hoverColor = activePalette.hex;
                }

                lineTspan.appendChild(subTspan);
                lineCursor = chunkEndIndexInLine;
            }
            textElement.appendChild(lineTspan);
            charCursor = lineStartIndex + lineText.length;
        });
    }

    group.appendChild(textElement);
    group.setAttribute('transform', `translate(${layout.x}, ${layout.y})`);
    svg.appendChild(group);
}


/**
 * Low-level function to create the SVG connector elements (base and highlight paths).
 */
function emitConnector(svg: SVGSVGElement, pathData: string, childData: AdjacencyNode, commonCardNames: string[], startX: number, startY: number, endX: number, endY: number, paletteMap: Map<string, DeterministicPalette>, containerId: string): void {
    const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
    group.dataset.sourceKeys = JSON.stringify(commonCardNames);
    group.id = `group-conn-${containerId}-${childData.id}`;

    const basePath = document.createElementNS("http://www.w3.org/2000/svg", "path");
    basePath.setAttribute('class', 'connector-path base-layer');
    basePath.setAttribute('d', pathData);

    const highlightPath = basePath.cloneNode() as SVGPathElement;
    highlightPath.classList.remove('base-layer');
    highlightPath.setAttribute('class', 'highlight-overlay');

    if (commonCardNames.length > 0) {
        const defs = svg.querySelector('defs');
        if (defs) {
            const idSuffix = `${containerId}-${childData.id}`;
            const baseGradientId = `grad-conn-base-${idSuffix}`;
            const highlightGradientId = `grad-conn-highlight-${idSuffix}`;

            const createGradient = (id: string, colorProp: 'hex' | 'hexSat') => {
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
    svg.insertBefore(group, svg.firstChild);
}

/**
 * Creates and appends a rounded SVG path to connect a parent and child node.
 */
export function createRoundedConnector(svg: SVGSVGElement, parentData: any, childData: AdjacencyNode, direction: number, config: NodeConfig, paletteMap: Map<string, DeterministicPalette>, allCards: Set<string>, containerId: string): void {
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

    let pathData: string;
    if (verticalOffset < 1e-6) {
        pathData = `M ${startX} ${y1} L ${endX} ${y2}`;
    } else {
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

    const parentKeys = parentData.id === 'main-anchor' ? allCards : (parentData.sourceKeysSet || new Set<string>());
    const childKeys = childData.sourceKeysSet || new Set<string>();
    const commonKeys = [...childKeys].filter(key => parentKeys.has(key));

    emitConnector(svg, pathData, childData, commonKeys, startX, y1, endX, y2, paletteMap, containerId);
}

/**
 * Recursively draws all nodes and their connectors for a given tree.
 */
export function drawNodesAndConnectors(svg: SVGSVGElement, nodes: AdjacencyNode[], parentData: any, direction: number, config: NodeConfig, paletteMap: Map<string, DeterministicPalette>, allCards: Set<string>, containerId: string): void {
    if (!nodes) return;
    for (const node of nodes) {
        createRoundedConnector(svg, parentData, node, direction, config, paletteMap, allCards, containerId);
        createNode(svg, node, true, config, paletteMap, containerId);
        if (node.children) {
            drawNodesAndConnectors(svg, node.children, node, direction, config, paletteMap, allCards, containerId);
        }
    }
}