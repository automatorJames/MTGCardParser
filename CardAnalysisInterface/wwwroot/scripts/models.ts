export interface DeterministicPalette {
    hex: string;
    hexLight: string;
    hexDark: string;
    hexSat: string;
}

export interface NodeSegment {
    text: string;
    // Assuming tokenType is not strictly needed on the client, otherwise define it
    tokenType: any;
}

export interface AdjacencyNode {
    id: string;
    segments: NodeSegment[];
    sourceOccurrenceKeys: string[];
    children: AdjacencyNode[];
    text: string;

    // Properties added dynamically, now explicitly typed
    dynamicHeight: number;
    wrappedLines: string[];
    lineHeight: number;
    childrenLayout: AdjacencyNode[];
    layout: { x: number; y: number };
}

export interface AnalyzedSpan {
    text: string;
    precedingAdjacencies: AdjacencyNode[];
    followingAdjacencies: AdjacencyNode[];
    cardColors: { [key: string]: string };
    positionalPalette: { [key: number]: DeterministicPalette };
    containingCards: string[];
}