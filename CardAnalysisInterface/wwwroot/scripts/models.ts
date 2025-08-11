// models.ts

/**
 * A standard, deterministic color palette.
 */
export interface DeterministicPalette {
    hex: string;
    hexLight: string;
    hexDark: string;
    hexSat: string;
}

/**
 * A single, non-divisible segment within a consolidated AdjacencyNode.
 */
export interface NodeSegment {
    text: string;
    palette: DeterministicPalette;
}

/**
 * Represents a node in the adjacency tree, including properties added
 * dynamically by the client for layout and rendering efficiency.
 */
export interface AdjacencyNode {
    // --- Properties from Server ---
    id: string;
    segments: NodeSegment[];
    sourceOccurrenceKeys: string[];
    children: AdjacencyNode[];
    text: string;

    // --- Properties added by Client ---

    // Added ONCE during initial processing for renderer efficiency
    sourceKeysSet?: Set<string>;

    // Added during layout calculation
    dynamicHeight: number;
    wrappedLines: string[];
    lineHeight: number;
    childrenLayout: AdjacencyNode[];
    layout: { x: number; y: number };
}

/**
 * This interface matches the raw JSON payload from the C# server (the DTO).
 * It uses plain objects and arrays that are easily serializable.
 */
export interface AnalyzedSpan {
    text: string;
    precedingAdjacencies: AdjacencyNode[];
    followingAdjacencies: AdjacencyNode[];
    cardPalettes: { [cardName: string]: DeterministicPalette };
    containingCards: string[];
    keyToPaletteMap: { [key: string]: DeterministicPalette };
    allKeys: string[];
    cardNameToKeysMap: { [cardName: string]: string[] };
}

/**
 * This interface represents the fully processed, in-memory data structure
 * optimized for rendering. It uses Map and Set for efficient lookups and is
 * generated once when the data is received.
 */
export interface ProcessedAnalyzedSpan extends Omit<AnalyzedSpan, 'keyToPaletteMap' | 'allKeys'> {
    keyToPaletteMap: Map<string, DeterministicPalette>;
    allKeys: Set<string>;
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