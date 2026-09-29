namespace Glyphotype.Tests.Infrastructure;

/// <summary>
/// One corpus document and the captures it must produce: one <see cref="GlyphSignature"/> line per
/// non-blank line of <see cref="Text"/>, in order.
/// </summary>
/// <param name="Name">
/// The document's name, which <see cref="IDocument.GetFormattedLines"/> replaces with <c>{this}</c> wherever
/// it appears in the text. Documents that don't exercise <c>{this}</c> share <see cref="Unnamed"/>.
/// </param>
public record TestDocument(string Name, string Text, string[] ExpectedLines) : IDocument
{
    /// <summary>A name that never occurs in any corpus text, so no <c>{this}</c> substitution happens.</summary>
    public const string Unnamed = "(unnamed)";

    /// <summary>The corpus section this document belongs to - shown alongside it in the test runner.</summary>
    public string Feature { get; init; }
}
