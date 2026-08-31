/**
 * The client shape of Glyphotype.Colors.HexPalette. Property names are the camel-cased record
 * members, which is what System.Text.Json's web defaults emit.
 */
export interface HexPalette {
    normal: string;
    light: string;
    dark: string;
    sat: string;
}

/** Which palette variant a gradient is drawn from: resting color, or the saturated hover color. */
export type PaletteVariant = 'normal' | 'sat';

/** One run of node text sharing a single capturing Glyph type (or none, for raw unmatched words). */
export interface TextChunk {
    text: string;
    glyphType: string | null;
}

/** One physically-wrapped line of node text, pre-split into its glyph-typed runs. */
export interface WrappedLine {
    text: string;
    chunks: TextChunk[];
}

/**
 * Represents a node in the adjacency tree, including properties added
 * dynamically by the client for layout and rendering efficiency.
 */
export interface AdjacencyNode {
    // --- Properties from Server ---
    id: string;
    text: string;
    /**
     * Character start index within `text` -> the friendly name of the top-level Glyph type that
     * captured the run beginning there, or null where nothing captured it. Names index into
     * {@link AnalyzedSpan.glyphPalettes}. Null for a node made entirely of uncaptured words.
     */
    spanGlyphTypes: { [startIndex: number]: string | null } | null;
    sourceOccurrenceDocumentNames: string[];
    children: AdjacencyNode[];

    // --- Properties added by Client ---
    sourceKeysSet?: Set<string>; // For efficient lookups (Set of DocumentNames)
    dynamicHeight: number;
    wrappedLines: WrappedLine[];
    lineHeight: number;
    childrenLayout: AdjacencyNode[];
    layout: { x: number; y: number };
}

/**
 * The central span itself, laid out like an {@link AdjacencyNode} so the same measuring, drawing
 * and connector code can treat it as one without a parallel code path.
 */
export interface AnchorNode {
    id: 'main-anchor';
    text: string;
    spanGlyphTypes: null;
    dynamicHeight: number;
    wrappedLines: WrappedLine[];
    lineHeight: number;
    layout: { x: number; y: number };
}

/** Anything the layout/draw code can measure and position. */
export type LayoutNode = AdjacencyNode | AnchorNode;

/**
 * This interface matches the raw JSON payload from the C# server (the DTO).
 */
export interface AnalyzedSpan {
    text: string;
    precedingAdjacencies: AdjacencyNode[];
    followingAdjacencies: AdjacencyNode[];
    /** Document name -> palette. Positional hues, meaningful only within this one card. */
    documentPalettes: { [documentName: string]: HexPalette };
    containingDocuments: string[];
    /** Glyph type name -> palette. A second positional rainbow, brightened and desaturated. */
    glyphPalettes: { [glyphTypeName: string]: HexPalette };
    containingGlyphTypes: string[];
}

/**
 * This interface represents the fully processed, in-memory data structure
 * optimized for rendering. It uses Map and Set for efficient lookups.
 */
export interface ProcessedAnalyzedSpan extends Omit<AnalyzedSpan, 'documentPalettes' | 'glyphPalettes'> {
    documentPalettes: Map<string, HexPalette>;
    glyphPalettes: Map<string, HexPalette>;
    allDocumentsSet: Set<string>;
    /**
     * Glyph type name -> every document whose throughline passes through a node that references
     * that type. Precomputed once per card so hovering a glyph key is a single map lookup rather
     * than a walk of the tree.
     */
    glyphTypeDocuments: Map<string, Set<string>>;
}

/**
 * The elements one card's hover handling touches, collected once per render rather than
 * re-queried on every pointer move.
 */
export interface CardHoverIndex {
    keyedGroups: KeyedGroupElement[];
    /** Every text run in the tree, captured or not. */
    textSpans: SVGElement[];
    documentChips: HTMLElement[];
    glyphChips: HTMLElement[];
    statAbove: SVGTextElement | null;
    statBelow: SVGTextElement | null;
}

/**
 * A custom HTMLElement type for the main card, allowing for data attachment.
 */
export type CardElement = HTMLElement & {
    __data?: ProcessedAnalyzedSpan;
    __hoverIndex?: CardHoverIndex;
    /** Signature of the hover state currently applied, so a repeat mouseover is a no-op. */
    __hoverSignature?: string;
};

/**
 * An SVG `<g>` that belongs to one or more documents - a node group or a connector group. The
 * document keys and the group's saturated highlight gradient are attached directly rather than
 * round-tripped through data attributes, so hover handling never has to parse JSON or rebuild a
 * gradient id out of the element's own id.
 */
export type KeyedGroupElement = SVGGElement & {
    __sourceKeys: string[];
    __sourceKeysSet: Set<string>;
    __highlightGradient: SVGLinearGradientElement | null;
    /** The key list the highlight gradient's stops were last built from, to skip redundant rebuilds. */
    __gradientKeys?: string;
    /**
     * Which vertical slot this group sits in, as `${direction}:${depth}` - node groups only.
     * Counting highlighted groups per column is what yields a throughline count (see
     * echo-tree-event-handler.ts).
     */
    __column?: string;
};

/**
 * Manages the state for a ResizeObserver instance tied to a echo tree container.
 */
export interface EchoTreeObserver {
    observer: ResizeObserver;
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
    /** Vertical distance from the anchor's edge to the participation-count labels above/below it. */
    statLabelGap: number;
}

/**
 * Everything the drawing pass needs that doesn't vary per node - passed as one object so node,
 * connector and text drawing don't each thread five positional arguments through.
 */
export interface RenderContext {
    svg: SVGSVGElement;
    defs: SVGDefsElement;
    config: NodeConfig;
    containerId: string;
    data: ProcessedAnalyzedSpan;
}
