// word-tree-layout-calculator.ts

import { AdjacencyNode, NodeConfig } from './models.js';

const columnIndexMap = new WeakMap<AdjacencyNode, number>();
const fanDeltaMap = new WeakMap<AdjacencyNode, number>();

/**
 * Retrieves the stored column index for a given node.
 */
export function getColumnIndex(node: AdjacencyNode): number {
    return columnIndexMap.get(node) ?? 0;
}

/**
 * Retrieves the stored fan-out delta for a given node's connector.
 */
export function getFanDelta(node: AdjacencyNode): number {
    return fanDeltaMap.get(node) || 0;
}

/**
 * Calculates display metrics for a single node based on its text content.
 */
export function getNodeMetrics(text: string, config: NodeConfig, svg: SVGSVGElement): { dynamicHeight: number, wrappedLines: string[], lineHeight: number } {
    const tempText = document.createElementNS("http://www.w3.org/2000/svg", "text");
    tempText.setAttribute('class', 'node-text');
    // Temporarily append to DOM for measurement
    svg.appendChild(tempText);

    const words = String(text || '').split(' ');
    const availableWidth = config.nodeWidth - config.nodePadding * 2;
    const lineHeight = 14;
    let currentLine = '';
    const wrappedLines: string[] = [];

    for (const word of words) {
        const testLine = currentLine ? `${currentLine} ${word}` : word;
        tempText.textContent = testLine;
        if (tempText.getComputedTextLength() > availableWidth && currentLine) {
            wrappedLines.push(currentLine);
            currentLine = word;
        } else {
            currentLine = testLine;
        }
    }
    wrappedLines.push(currentLine);
    svg.removeChild(tempText); // Clean up

    const totalTextHeight = wrappedLines.length * lineHeight;
    const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + config.nodePadding * 2);
    return { dynamicHeight, wrappedLines, lineHeight };
}

/**
 * Recursively calculates and attaches display metrics to each node in a tree.
 */
export function preCalculateAllNodeMetrics(node: any, config: NodeConfig, svg: SVGSVGElement): void {
    if (!node) return;
    const metrics = getNodeMetrics(node.text, config, svg);
    Object.assign(node, metrics); // Assign metrics to the node
    if (node.children) {
        node.children.forEach((child: AdjacencyNode) => preCalculateAllNodeMetrics(child, config, svg));
    }
}

/**
 * Recursively calculates the layout positions for a tree of nodes.
 */
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
export function computeFanDeltasAndColumnPush(rootNodes: AdjacencyNode[], anchorX: number, anchorY: number, config: NodeConfig): Map<number, number> {
    const { fanGap } = config;
    const columnStats = new Map<number, { maxUp: number; maxDown: number }>();

    const recordFanStats = (columnIndex: number, upCount: number, downCount: number) => {
        const currentStats = columnStats.get(columnIndex) ?? { maxUp: 0, maxDown: 0 };
        columnStats.set(columnIndex, {
            maxUp: Math.max(currentStats.maxUp, upCount),
            maxDown: Math.max(currentStats.maxDown, downCount)
        });
    };

    type ParentLike = { layout: { x: number; y: number }, children?: AdjacencyNode[] };

    const processChildren = (children: AdjacencyNode[] = [], parent: ParentLike) => {
        const up: AdjacencyNode[] = [], down: AdjacencyNode[] = [], flat: AdjacencyNode[] = [];
        for (const child of children) {
            const deltaY = child.layout.y - parent.layout.y;
            if (Math.abs(deltaY) < 1e-6) flat.push(child);
            else if (deltaY > 0) down.push(child);
            else up.push(child);
        }

        const assignGroupDeltas = (nodeArray: AdjacencyNode[], kind: 'up' | 'down') => {
            if (nodeArray.length === 0) return;

            // *** FIX: Sort by distance from parent, FARTHEST first. ***
            // This makes the outermost node get the smallest delta (index 0), so it peels off first.
            nodeArray.sort((a, b) => Math.abs(b.layout.y - parent.layout.y) - Math.abs(a.layout.y - parent.layout.y));

            // Assign delta based on index: farthest gets δ=0, nearest gets δ=(n-1)*fanGap
            nodeArray.forEach((child, index) => fanDeltaMap.set(child, index * fanGap));

            // Record the number of fanning children for this group
            const columnIndex = getColumnIndex(nodeArray[0]) ?? 0;
            if (kind === 'up') recordFanStats(columnIndex, nodeArray.length, 0);
            else recordFanStats(columnIndex, 0, nodeArray.length);

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

    const pseudoParent: ParentLike = { layout: { x: anchorX, y: anchorY }, children: rootNodes };
    processChildren(rootNodes, pseudoParent);

    const columnPush = new Map<number, number>();
    columnStats.forEach((stats, columnIndex) => {
        // The push required is driven by the node with the most children in the column.
        // The max delta is (n-1)*fanGap. This is the amount the column must be pushed out.
        const maxFanIndex = Math.max(0, Math.max(stats.maxUp, stats.maxDown) - 1);
        columnPush.set(columnIndex, maxFanIndex * fanGap);
    });
    return columnPush;
}