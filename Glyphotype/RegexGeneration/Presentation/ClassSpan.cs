namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// A single contiguous run of text within a <see cref="ClassLine"/> of <see cref="GlyphClassRenderer"/>'s
/// rendered C# class output - the class-view analog of <see cref="SmartSpan"/>.
/// </summary>
/// <param name="Content">The literal text this span renders.</param>
/// <param name="DataPath">
/// The fully qualified name of the graph node this span represents, for hover cross-highlighting - null for
/// spans with no corresponding named group (neutral C# syntax, literal Nib text), which never participate
/// in the hover system at all, the same way a formatted regex's own literal-match/joiner spans don't.
/// </param>
/// <param name="Palette">The style palette (color plus bold/italic flags) this span is rendered with.</param>
/// <param name="NavigateTo">
/// The node to navigate the C# Class view to when this span is clicked directly - null for a span that
/// isn't click-navigable to a single fixed target, either because it carries no <see cref="DataPath"/> at
/// all, because it names a type with no class file of its own to descend into (an enum, a primitive, an
/// unbound generic like <c>OneOf&lt;,&gt;</c>), or because it's a <see cref="Resolutions"/> span instead
/// (a <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/> capture has no *one* fixed target).
/// </param>
/// <param name="Resolutions">
/// For a <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/> property's type-name or property-name span:
/// every concrete type that capture actually resolved to somewhere in the corpus, offered as a click-to-pick
/// menu instead of a single direct navigation - null (and <see cref="NavigateTo"/> also null) for every
/// other span, including a <see cref="Glyphotype.GlyphPrimitives.DynamicGlyph"/> property that never resolved
/// to anything (nothing to pick from).
/// </param>
public record ClassSpan(string Content, string DataPath, SpanStylePalette Palette, NamedGroupNode NavigateTo = null, IReadOnlyList<DynamicResolutionOption> Resolutions = null)
{
    public override string ToString() => Content;
}
