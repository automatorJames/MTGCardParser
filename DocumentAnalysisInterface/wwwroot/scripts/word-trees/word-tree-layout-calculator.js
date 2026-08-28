const columnIndexMap = new WeakMap();
const fanDeltaMap = new WeakMap();
/**
 * Retrieves the stored column index for a given node.
 */
export function getColumnIndex(node) {
    return columnIndexMap.get(node) ?? 0;
}
/**
 * Retrieves the stored fan-out delta for a given node's connector.
 */
export function getFanDelta(node) {
    return fanDeltaMap.get(node) || 0;
}
/**
 * The node's glyph-type boundaries as a sorted list. Empty when nothing in the node was captured,
 * which lets every caller take a single-chunk fast path.
 */
function getGlyphStops(node) {
    const spanGlyphTypes = node.spanGlyphTypes;
    if (!spanGlyphTypes)
        return [];
    return Object.entries(spanGlyphTypes)
        .map(([index, glyphType]) => ({ index: parseInt(index, 10), glyphType }))
        .sort((a, b) => a.index - b.index);
}
/**
 * Splits one physically-wrapped line into runs of uniform glyph type. The single place that walks
 * the stop list, shared by measurement (where captured runs render bold and so measure wider) and
 * by drawing (where each run becomes its own tspan).
 * @param lineText The line's text.
 * @param lineStartIndex The line's start index within the node's full text - stops are absolute.
 */
export function chunkLine(lineText, lineStartIndex, stops) {
    if (stops.length === 0 || lineText.length === 0)
        return [{ text: lineText, glyphType: null }];
    const chunks = [];
    let cursor = 0;
    while (cursor < lineText.length) {
        const absoluteCursor = lineStartIndex + cursor;
        let activeGlyphType = null;
        let nextStopAbsoluteIndex = lineStartIndex + lineText.length;
        for (const stop of stops) {
            if (stop.index <= absoluteCursor)
                activeGlyphType = stop.glyphType;
            else {
                nextStopAbsoluteIndex = stop.index;
                break;
            }
        }
        // Guaranteed to advance: nextStopAbsoluteIndex is strictly past absoluteCursor.
        const chunkEnd = Math.min(lineText.length, nextStopAbsoluteIndex - lineStartIndex);
        chunks.push({ text: lineText.substring(cursor, chunkEnd), glyphType: activeGlyphType });
        cursor = chunkEnd;
    }
    return chunks;
}
/**
 * Fills a scratch <text> element with one tspan per chunk so it can be measured. Only the bold
 * flag matters here - captured runs render bold and are therefore wider than the same characters
 * unstyled, which is exactly what the wrap point depends on.
 */
function populateTspansForMeasurement(textEl, chunks) {
    textEl.innerHTML = '';
    for (const chunk of chunks) {
        const tspan = document.createElementNS("http://www.w3.org/2000/svg", "tspan");
        tspan.textContent = chunk.text;
        if (chunk.glyphType)
            tspan.style.fontWeight = 'bold';
        textEl.appendChild(tspan);
    }
}
/**
 * Calculates display metrics for a single node based on its text content,
 * accounting for bold styling which affects text width.
 *
 * Each wrapped line keeps the glyph-typed chunks it was measured from, so the drawing pass reuses
 * that split instead of recovering each line's offset with an indexOf of its own text (which
 * silently picks the wrong offset whenever a line repeats earlier in the node).
 */
export function getNodeMetrics(node, config, svg) {
    const tempText = document.createElementNS("http://www.w3.org/2000/svg", "text");
    tempText.setAttribute('class', 'node-text');
    svg.appendChild(tempText);
    const text = String(node.text || '');
    const stops = getGlyphStops(node);
    const words = text.split(' ');
    const availableWidth = config.nodeWidth - config.nodePadding * 2;
    const lineHeight = 14;
    const wrappedLines = [];
    let currentLine = '';
    let lineStartIndex = 0;
    let currentWordAbsoluteIndex = 0;
    const pushLine = (lineText, startIndex) => wrappedLines.push({ text: lineText, chunks: chunkLine(lineText, startIndex, stops) });
    for (const word of words) {
        const testLine = currentLine ? `${currentLine} ${word}` : word;
        populateTspansForMeasurement(tempText, chunkLine(testLine, lineStartIndex, stops));
        if (tempText.getComputedTextLength() > availableWidth && currentLine) {
            pushLine(currentLine, lineStartIndex);
            lineStartIndex = currentWordAbsoluteIndex;
            currentLine = word;
        }
        else {
            currentLine = testLine;
        }
        // Advance the absolute index by word length plus a space
        currentWordAbsoluteIndex += word.length + 1;
    }
    pushLine(currentLine, lineStartIndex);
    svg.removeChild(tempText); // Clean up
    const totalTextHeight = wrappedLines.length * lineHeight;
    const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + config.nodePadding * 2);
    return { dynamicHeight, wrappedLines, lineHeight };
}
/**
 * Recursively calculates and attaches display metrics to each node in a tree.
 */
export function preCalculateAllNodeMetrics(node, config, svg) {
    if (!node)
        return;
    Object.assign(node, getNodeMetrics(node, config, svg));
    const children = node.children;
    if (children)
        children.forEach(child => preCalculateAllNodeMetrics(child, config, svg));
}
/**
 * Recursively calculates the layout positions for a tree of nodes.
 */
export function calculateLayout(nodes, depth, parentX, parentY, direction, config) {
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
        // The offset from parent center to child center
        const offset = (config.nodeWidth / 2) + config.hGap + (config.nodeWidth / 2);
        const nodeX = parentX + (direction * offset);
        const nodeY = currentY + effectiveHeight / 2;
        node.layout = { x: nodeX, y: nodeY };
        columnIndexMap.set(node, depth + 1);
        layoutInfo.push(node);
        // Position children relative to the current node
        for (const childNode of node.childrenLayout) {
            childNode.layout.x += nodeX;
            childNode.layout.y += nodeY;
        }
        layoutInfo.push(...node.childrenLayout);
        currentY += effectiveHeight + config.vGap;
    }
    return { layout: layoutInfo, totalHeight: totalGroupHeight };
}
/**
 * Calculates per-connector fan deltas (δ) and computes the required outward push for each column.
 * This prevents connectors from overlapping when fanning out from a single parent.
 * @returns A map of `columnIndex -> requiredPush`.
 */
export function computeFanDeltasAndColumnPush(rootNodes, anchorX, anchorY, config) {
    const { fanGap } = config;
    const columnStats = new Map();
    const recordFanStats = (columnIndex, upCount, downCount) => {
        const currentStats = columnStats.get(columnIndex) ?? { maxUp: 0, maxDown: 0 };
        columnStats.set(columnIndex, {
            maxUp: Math.max(currentStats.maxUp, upCount),
            maxDown: Math.max(currentStats.maxDown, downCount)
        });
    };
    const processChildren = (children = [], parent) => {
        const up = [], down = [], flat = [];
        for (const child of children) {
            const deltaY = child.layout.y - parent.layout.y;
            if (Math.abs(deltaY) < 1e-6)
                flat.push(child);
            else if (deltaY > 0)
                down.push(child);
            else
                up.push(child);
        }
        const assignGroupDeltas = (nodeArray, kind) => {
            if (nodeArray.length === 0)
                return;
            // *** FIX: Sort by distance from parent, FARTHEST first. ***
            // This makes the outermost node get the smallest delta (index 0), so it peels off first.
            nodeArray.sort((a, b) => Math.abs(b.layout.y - parent.layout.y) - Math.abs(a.layout.y - parent.layout.y));
            // Assign delta based on index: farthest gets δ=0, nearest gets δ=(n-1)*fanGap
            nodeArray.forEach((child, index) => fanDeltaMap.set(child, index * fanGap));
            // Record the number of fanning children for this group
            const columnIndex = getColumnIndex(nodeArray[0]) ?? 0;
            if (kind === 'up')
                recordFanStats(columnIndex, nodeArray.length, 0);
            else
                recordFanStats(columnIndex, 0, nodeArray.length);
            // Recurse
            nodeArray.forEach(child => processChildren(child.children, child));
        };
        assignGroupDeltas(up, 'up');
        assignGroupDeltas(down, 'down');
        // Flat connectors don't fan, but their children might.
        flat.forEach(child => {
            fanDeltaMap.set(child, 0);
            processChildren(child.children, child);
        });
    };
    const pseudoParent = { layout: { x: anchorX, y: anchorY }, children: rootNodes };
    processChildren(rootNodes, pseudoParent);
    const columnPush = new Map();
    columnStats.forEach((stats, columnIndex) => {
        // The push required is driven by the node with the most children in the column.
        // The max delta is (n-1)*fanGap. This is the amount the column must be pushed out.
        const maxFanIndex = Math.max(0, Math.max(stats.maxUp, stats.maxDown) - 1);
        columnPush.set(columnIndex, maxFanIndex * fanGap);
    });
    return columnPush;
}
//# sourceMappingURL=word-tree-layout-calculator.js.map