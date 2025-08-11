// word-tree-layout-calculator.ts

import { AdjacencyNode } from './models.js';
import { NodeConfig } from './word-tree-svg-drawer.js';

// WeakMaps to store layout-specific metadata without modifying the AdjacencyNode interface.
const columnIndexMap = new WeakMap<AdjacencyNode, number>(); // Stores the 1-based column index of a node.
const fanDeltaMap = new WeakMap<AdjacencyNode, number>();   // Stores the extra horizontal length for fanning connectors.

/**
 * Retrieves the stored column index for a given node.
 * @param node The node to query.
 * @returns The 1-based column index, or 0 if not found.
 */
export function getColumnIndex(node: AdjacencyNode): number {
    return columnIndexMap.get(node) ?? 0;
}

/**
 * Retrieves the stored fan-out delta for a given node's connector.
 * @param node The node whose connector fan-out is needed.
 * @returns The calculated fan-out delta in pixels.
 */
export function getFanDelta(node: AdjacencyNode): number {
    return fanDeltaMap.get(node) || 0;
}

/**
 * Calculates the display metrics for a single node based on its text content and configuration.
 * @param text The text content of the node.
 * @param config The rendering configuration.
 * @param svg The parent SVG element, used for text measurement.
 * @returns An object containing the calculated dynamic height, wrapped text lines, and line height.
 */
export function getNodeMetrics(text: string, config: NodeConfig, svg: SVGSVGElement): { dynamicHeight: number, wrappedLines: string[], lineHeight: number } {
    const nodeText = String(text || '');
    const { nodeWidth, nodePadding } = config;
    const fontSize = 12;
    const lineHeight = 14;
    const availableWidth = nodeWidth - nodePadding * 2;

    const tempText = document.createElementNS("http://www.w3.org/2000/svg", "text");
    tempText.setAttribute('class', 'node-text');
    tempText.style.fontSize = `${fontSize}px`;
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
    const dynamicHeight = Math.max(config.nodeHeight, totalTextHeight + nodePadding * 2);
    return { dynamicHeight, wrappedLines, lineHeight };
}

/**
 * Recursively calculates and attaches display metrics to each node in a tree.
 * @param node The root node to start processing from.
 * @param config The rendering configuration.
 * @param svg The parent SVG element for measurement.
 */
export function preCalculateAllNodeMetrics(node: any, config: NodeConfig, svg: SVGSVGElement): void {
    if (!node) return;
    const metrics = getNodeMetrics(node.text, config, svg);
    node.dynamicHeight = metrics.dynamicHeight;
    node.wrappedLines = metrics.wrappedLines;
    node.lineHeight = metrics.lineHeight;
    if (node.children) {
        node.children.forEach((child: AdjacencyNode) => preCalculateAllNodeMetrics(child, config, svg));
    }
}

/**
 * Recursively calculates the layout positions for a tree of nodes.
 * @param nodes The array of nodes at the current level.
 * @param depth The current recursion depth.
 * @param parentX The x-coordinate of the parent node.
 * @param parentY The y-coordinate of the parent node.
 * @param direction The direction of layout (-1 for preceding, 1 for following).
 * @param config The rendering configuration.
 * @returns An object containing the flat list of all laid-out nodes and the total height of the group.
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
        const offset = (config.nodeWidth / 2) + config.hGap + (config.nodeWidth / 2);
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

/**
 * Calculates the necessary "push" for each column to prevent connector overlaps from fanning out.
 * It also assigns a fan-out delta (δ) to each node's connector.
 * @param rootNodes The root nodes of the tree (e.g., precedingAdjacencies).
 * @param anchorX The x-coordinate of the main anchor span.
 * @param anchorY The y-coordinate of the main anchor span.
 * @param config The rendering configuration.
 * @returns A map of column index to the required outward push in pixels.
 */
export function computeColumnOffsetsAndAssignFan(rootNodes: AdjacencyNode[], anchorX: number, anchorY: number, config: NodeConfig): Map<number, number> {
    const fanGap = Math.max(0, config.fanGap ?? 0);
    const columnStats = new Map<number, { maxUp: number; maxDown: number }>();

    const recordFanStats = (columnIndex: number, upCount: number, downCount: number) => {
        const currentStats = columnStats.get(columnIndex) ?? { maxUp: 0, maxDown: 0 };
        if (upCount > currentStats.maxUp) currentStats.maxUp = upCount;
        if (downCount > currentStats.maxDown) currentStats.maxDown = downCount;
        columnStats.set(columnIndex, currentStats);
    };

    type ParentLike = { layout: { x: number; y: number }, children?: AdjacencyNode[] };

    const processChildren = (children: AdjacencyNode[], parent: ParentLike) => {
        if (!children || children.length === 0) return;

        const up: AdjacencyNode[] = [];
        const down: AdjacencyNode[] = [];
        const flat: AdjacencyNode[] = [];

        for (const child of children) {
            const deltaY = child.layout.y - parent.layout.y;
            if (Math.abs(deltaY) < 1e-6) flat.push(child);
            else if (deltaY > 0) down.push(child);
            else up.push(child);
        }

        const byAbsDy = (a: AdjacencyNode, b: AdjacencyNode) =>
            Math.abs(a.layout.y - parent.layout.y) - Math.abs(b.layout.y - parent.layout.y);

        const assignGroup = (nodeArray: AdjacencyNode[], kind: 'up' | 'down') => {
            if (nodeArray.length === 0) return;
            nodeArray.sort(byAbsDy); // Sort by distance from parent, nearest first.

            const nodeCount = nodeArray.length;
            for (let index = 0; index < nodeCount; index++) {
                const child = nodeArray[index];
                const delta = index * fanGap; // nearest δ=0, farthest δ=(n-1)*fanGap
                fanDeltaMap.set(child, delta);
            }

            const columnIndex = getColumnIndex(nodeArray[0]) ?? 0;
            if (kind === 'up') recordFanStats(columnIndex, nodeCount, 0);
            else recordFanStats(columnIndex, 0, nodeCount);

            for (const child of nodeArray) processChildren(child.children, child);
        };

        assignGroup(up, 'up');
        assignGroup(down, 'down');

        for (const child of flat) {
            fanDeltaMap.set(child, 0); // No fanning for straight connectors
            processChildren(child.children, child);
        }
    };

    const pseudoParent: ParentLike = { layout: { x: anchorX, y: anchorY }, children: rootNodes };
    processChildren(rootNodes, pseudoParent);

    const columnPushRaw = new Map<number, number>();
    columnStats.forEach((stats, columnIndex) => {
        const neededPush = fanGap * Math.max(stats.maxUp, stats.maxDown);
        columnPushRaw.set(columnIndex, neededPush);
    });
    return columnPushRaw;
}