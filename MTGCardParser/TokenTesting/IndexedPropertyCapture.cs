namespace MTGCardParser.TokenTesting; // Or your preferred namespace

/// <summary>
/// Represents a property capture from a token, enriched with a stable index
/// for consistent processing (e.g., coloring) and ordered by position.
/// </summary>
/// <param name="Property">The unified property capture definition.</param>
/// <param name="Span">The text span of the capture.</param>
/// <param name="OriginalIndex">A stable, zero-based index of this capture within its parent token's original list of properties.</param>
public record IndexedPropertyCapture(PropertyCapture Property, TextSpan Span, int OriginalIndex);