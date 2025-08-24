// type-expression-models.ts

/**
 * Represents RegexDTOs.RegexCapturePosition
 */
export interface RegexCapturePosition {
    Capture: string;
    Start: number;
    End: number;
}

/**
 * Represents a C# Dictionary KeyValuePair for JSON serialization.
 */
export interface KeyValuePair<TKey, TValue> {
    Key: TKey;
    Value: TValue;
}

/**
 * Represents RegexDTOs.ValueCaptureVariantSet
 */
export interface ValueCaptureVariantSet {
    CanonicalRepresentation: string;
    TotalCount: number;
    VariantCounts: KeyValuePair<RegexCapturePosition, number>[];
}

/**
 * Represents RegexDTOs.RegexPropValueSet
 */
export interface RegexPropValueSet {
    PropPathNameFormatted: string;
    CaptureGroupPositionStart: number;
    CaptureGroupPositionEnd: number;
    ValueCaptureCounts: ValueCaptureVariantSet[];
}

/**
 * Represents the main DTO for the Type Expressions page: RegexDTOs.TokenUnitCapture
 */
export interface TokenUnitCapture {
    Type: { Name: string };
    OccurrenceCount: number;
    RegexString: string;
    RegexPropValueSets: RegexPropValueSet[];
    Palette: { Hex: string };
}