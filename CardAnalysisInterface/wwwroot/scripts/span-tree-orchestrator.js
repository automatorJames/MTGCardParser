// span-tree-orchestrator.ts
import * as Layout from "./word-tree-layout-calculator.js";
import * as Drawer from "./word-tree-svg-drawer.js";
/**
 * Builds a cumulative map of layout offsets. For each column, the offset is the
 * sum of its own required push plus all pushes from columns closer to the center.
 * @param rawPushMap A map from a column index to its required push.
 * @param maxColumn The maximum column index to process.
 * @returns A new map with cumulative offset values.
 */
function buildCumulativeOffsets(rawPushMap, maxColumn) {
    const cumulativeMap = new Map();
    let accumulator = 0;
    for (let columnIndex = 1; columnIndex <= maxColumn; columnIndex++) {
        accumulator += rawPushMap.get(columnIndex) || 0;
        cumulativeMap.set(columnIndex, accumulator);
    }
    return cumulativeMap;
}
/**
 * Orchestrates the entire process of calculating layout and drawing a word tree SVG.
 */
export function orchestrateWordTreeRender(container) {
    const card = container.closest('.span-trees-card');
    const processedData = card?.__data;
    const svg = container.querySelector('svg');
    if (!processedData || !svg)
        return;
    svg.innerHTML = '<defs></defs>'; // Clear previous render
    const config = {
        nodeWidth: 200, nodePadding: 8, nodeHeight: 40, hGap: 40, vGap: 20,
        cornerRadius: 10, mainSpanFill: '#e0e0e0', mainSpanColor: "#e0e0e0",
        horizontalPadding: 20, gradientTransitionRatio: 0.1, fanGap: 24
    };
    const { cardPalettes, allCardsSet, text, precedingAdjacencies, followingAdjacencies } = processedData;
    const { width: availableWidth } = container.getBoundingClientRect();
    if (availableWidth <= 0)
        return;
    // 1. Pre-calculate metrics for all nodes
    const mainSpanObject = { text, id: 'main-anchor', layout: { x: 0, y: 0 } };
    Layout.preCalculateAllNodeMetrics(mainSpanObject, config, svg);
    [...precedingAdjacencies, ...followingAdjacencies].forEach(node => Layout.preCalculateAllNodeMetrics(node, config, svg));
    // 2. Calculate initial layout for both trees
    const precedingResult = Layout.calculateLayout(precedingAdjacencies, 0, 0, 0, -1, config);
    const followingResult = Layout.calculateLayout(followingAdjacencies, 0, 0, 0, 1, config);
    // 3. Center the layout vertically
    const totalHeight = Math.max(precedingResult.totalHeight, followingResult.totalHeight, mainSpanObject.dynamicHeight) + config.vGap * 2;
    const mainSpanY = totalHeight / 2;
    mainSpanObject.layout.y = mainSpanY;
    [...precedingResult.layout, ...followingResult.layout].forEach(node => node.layout.y += mainSpanY);
    // 4. Calculate fanning deltas and the required push for each column
    const precedingRawPush = Layout.computeFanDeltasAndColumnPush(precedingAdjacencies, 0, mainSpanY, config);
    const followingRawPush = Layout.computeFanDeltasAndColumnPush(followingAdjacencies, 0, mainSpanY, config);
    const maxColPreceding = precedingResult.layout.reduce((max, node) => Math.max(max, Layout.getColumnIndex(node)), 0);
    const maxColFollowing = followingResult.layout.reduce((max, node) => Math.max(max, Layout.getColumnIndex(node)), 0);
    const precedingOffsets = buildCumulativeOffsets(precedingRawPush, maxColPreceding);
    const followingOffsets = buildCumulativeOffsets(followingRawPush, maxColFollowing);
    // 5. Apply cumulative offsets to shift columns outward
    precedingResult.layout.forEach(node => {
        node.layout.x -= precedingOffsets.get(Layout.getColumnIndex(node)) || 0;
    });
    followingResult.layout.forEach(node => {
        node.layout.x += followingOffsets.get(Layout.getColumnIndex(node)) || 0;
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
    }
    else {
        const margin = (availableWidth - naturalTreeWidth) / 2;
        svg.setAttribute('viewBox', `${minX - margin} 0 ${availableWidth} ${totalHeight}`);
    }
    // 7. Draw the final SVG elements
    Drawer.drawNodesAndConnectors(svg, precedingAdjacencies, mainSpanObject, -1, config, cardPalettes, allCardsSet, container.id);
    Drawer.drawNodesAndConnectors(svg, followingAdjacencies, mainSpanObject, 1, config, cardPalettes, allCardsSet, container.id);
    Drawer.createNode(svg, mainSpanObject, false, config, cardPalettes, container.id);
}
//# sourceMappingURL=span-tree-orchestrator.js.map