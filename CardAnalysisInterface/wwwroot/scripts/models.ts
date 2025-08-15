/**
 * A standard, deterministic color palette.
 */
export interface DeterministicPalette {
    hex: string;
    hexSat: string;
    hexLight: string;
    seed?: string;
}

/**
 * Represents a node in the adjacency tree, including properties added
 * dynamically by the client for layout and rendering efficiency.
 */
export interface AdjacencyNode {
    // --- Properties from Server ---
    id: string;
    text: string;
    spanPalettes: { [startIndex: number]: DeterministicPalette } | null;
    sourceOccurrenceKeys: string[]; // Now contains CardNames
    children: AdjacencyNode[];

    // --- Properties added by Client ---
    sourceKeysSet?: Set<string>; // For efficient lookups (Set of CardNames)
    dynamicHeight: number;
    wrappedLines: string[];
    lineHeight: number;
    childrenLayout: AdjacencyNode[];
    layout: { x: number; y: number };
}

/**
 * This interface matches the raw JSON payload from the C# server (the DTO).
 */
export interface AnalyzedSpan {
    text: string;
    precedingAdjacencies: AdjacencyNode[];
    followingAdjacencies: AdjacencyNode[];
    cardPalettes: { [cardName: string]: DeterministicPalette };
    containingCards: string[];
}

/**
 * This interface represents the fully processed, in-memory data structure
 * optimized for rendering. It uses Map and Set for efficient lookups.
 */
export interface ProcessedAnalyzedSpan extends Omit<AnalyzedSpan, 'cardPalettes'> {
    cardPalettes: Map<string, DeterministicPalette>;
    allCardsSet: Set<string>;
}

/**
 * A custom HTMLElement type for the main card, allowing for data attachment.
 */
export type CardElement = HTMLElement & { __data?: ProcessedAnalyzedSpan };

/**
 * Manages the state for a ResizeObserver instance tied to a word tree container.
 */
export interface WordTreeObserver {
    observer: ResizeObserver;
    animationFrameId: number | null;
}

/**
 * An interface for an object that can manage an animation frame,
 * allowing animations to be started and stopped.
 */
export interface AnimationController {
    animationFrameId: number | null;
}

/**
 * Defines the configuration for node and tree rendering.
 */
export interface NodeConfig {
    nodeWidth: number;
    nodePadding: number;
    nodeHeight: number;
    hGap: number; // Horizontal gap between node edges
    vGap: number; // Vertical gap between node edges
    cornerRadius: number;
    mainSpanFill: string;
    mainSpanColor: string;
    horizontalPadding: number; // Padding at the far left/right of the SVG
    gradientTransitionRatio: number;
    fanGap: number; // Additional horizontal distance for each fanned-out connector
}