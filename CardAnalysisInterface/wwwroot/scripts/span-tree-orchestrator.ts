// span-tree-orchestrator.ts

import { CardElement, ProcessedAnalyzedSpan } from "./models.js";
import * as Layout from "./word-tree-layout-calculator.js";
import * as Drawer from "./word-tree-svg-drawer.js";

/**
 * Builds a cumulative map of values from an input map.
 * For each key `c`, the output value is the sum of all input values for keys `<= c`.
 * @param rawPushMap A map from a column index to a value.
 * @param maxColumn The maximum column index to iterate up to.
 * @returns A new map with cumulative values.
 */
function buildCumulativePush(rawPushMap: Map<number, number>, maxColumn: number): Map<number, number> {
    const cumulativeMap = new Map<number, number>();
    let accumulator = 0;
    for (let columnIndex = 1; columnIndex <= maxColumn; columnIndex++) {
        accumulator += rawPushMap.get(columnIndex) || 0;
        cumulativeMap.set(columnIndex, accumulator);
    }
    return cumulativeMap;
}

/**
 * Orchestrates the entire process of calculating layout and drawing a word tree SVG.
 * @param container The HTML element that will host the SVG.
 */
export function orchestrateWordTreeRender(container: HTMLElement): void {
    const card = container.closest<CardElement>('.span-trees-card');
    const processedData = card?.__data;
    const svg = container.querySelector('svg');
    if (!processedData || !svg) return;

    svg.innerHTML = '<defs></defs>'; // Clear previous render
    const config: Drawer.NodeConfig = {
        nodeWidth: 200, nodePadding: 8, nodeHeight: 40, hGap: 40, vGap: 20,
        cornerRadius: 10, mainSpanFill: '#e0e0e0', mainSpanColor: "#e0e0e0",
        horizontalPadding: 500, gradientTransitionRatio: 0.1, fanGap: 20
    };

    const { keyToPaletteMap, allKeys, text, precedingAdjacencies, followingAdjacencies } = processedData;
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0) return;

    // 1. Pre-calculate metrics for all nodes
    const mainSpanObject: any = { text, id: 'main-anchor', layout: { x: 0, y: 0 } };
    Layout.preCalculateAllNodeMetrics(mainSpanObject, config, svg);
    precedingAdjacencies.forEach(node => Layout.preCalculateAllNodeMetrics(node, config, svg));
    followingAdjacencies.forEach(node => Layout.preCalculateAllNodeMetrics(node, config, svg));

    // 2. Calculate initial layout for both trees
    const precedingResult = Layout.calculateLayout(precedingAdjacencies, 0, 0, 0, -1, config);
    const followingResult = Layout.calculateLayout(followingAdjacencies, 0, 0, 0, 1, config);

    // 3. Center the layout vertically
    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    const mainSpanY = totalHeight / 2;
    mainSpanObject.layout.y = mainSpanY;
    precedingResult.layout.forEach(node => node.layout.y += mainSpanY);
    followingResult.layout.forEach(node => node.layout.y += mainSpanY);

    // 4. Calculate fanning offsets and column pushes
    const precedingRawPush = Layout.computeColumnOffsetsAndAssignFan(precedingAdjacencies, 0, mainSpanY, config);
    const followingRawPush = Layout.computeColumnOffsetsAndAssignFan(followingAdjacencies, 0, mainSpanY, config);

    const maxColPreceding = precedingResult.layout.reduce((max, node) => Math.max(max, Layout.getColumnIndex(node)), 0);
    const maxColFollowing = followingResult.layout.reduce((max, node) => Math.max(max, Layout.getColumnIndex(node)), 0);

    const precedingPushCumulative = buildCumulativePush(precedingRawPush, maxColPreceding);
    const followingPushCumulative = buildCumulativePush(followingRawPush, maxColFollowing);

    // 5. Apply cumulative pushes to shift columns outward
    precedingResult.layout.forEach(node => {
        const col = Layout.getColumnIndex(node);
        node.layout.x -= precedingPushCumulative.get(col) || 0;
    });
    followingResult.layout.forEach(node => {
        const col = Layout.getColumnIndex(node);
        node.layout.x += followingPushCumulative.get(col) || 0;
    });

    // 6. Calculate final content bounds and set SVG viewBox for scaling
    let minX = -config.nodeWidth / 2;
    let maxX = config.nodeWidth / 2;
    [...precedingResult.layout, ...followingResult.layout].forEach(node => {
        minX = Math.min(minX, node.layout.x - config.nodeWidth / 2);
        maxX = Math.max(maxX, node.layout.x + config.nodeWidth / 2);
    });

    const naturalTreeWidth = maxX - minX;
    const naturalContentWidth = naturalTreeWidth + config.horizontalPadding * 2;
    container.style.height = `${totalHeight}px`;
    if (naturalContentWidth > availableWidth) {
        const scaleFactor = availableWidth / naturalContentWidth;
        container.style.height = `${totalHeight * scaleFactor}px`;
        svg.setAttribute('viewBox', `${minX - config.horizontalPadding} 0 ${naturalContentWidth} ${totalHeight}`);
    } else {
        const margin = (availableWidth - naturalTreeWidth) / 2;
        svg.setAttribute('viewBox', `${minX - margin} 0 ${availableWidth} ${totalHeight}`);
    }

    // 7. Draw the final SVG elements
    Drawer.drawNodesAndConnectors(svg, precedingAdjacencies, mainSpanObject, -1, config, keyToPaletteMap, allKeys, container.id);
    Drawer.drawNodesAndConnectors(svg, followingAdjacencies, mainSpanObject, 1, config, keyToPaletteMap, allKeys, container.id);
    Drawer.createNode(svg, mainSpanObject, 0, mainSpanY, false, config, keyToPaletteMap, container.id);
}