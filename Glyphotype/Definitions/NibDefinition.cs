namespace Glyphotype.Definitions;

/// <summary>
/// One entry of a <see cref="GlyphDefinition.Nibs"/> list - the portable counterpart of a <see cref="Nib"/>, and of
/// the <see cref="Glyph"/> helper that authors it: a plain string, <see cref="Glyph.Pattern"/>, <see cref="Glyph.Alt"/>,
/// <see cref="Glyph.Opt"/>, <see cref="Glyph.Plural"/>, <see cref="Glyph.Prop"/> or <see cref="Nib.This"/>. A property
/// nib refers to its property by name; the property itself is declared in <see cref="GlyphDefinition.Properties"/>.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$nib")]
[JsonDerivedType(typeof(Literal), "literal")]
[JsonDerivedType(typeof(Pattern), "pattern")]
[JsonDerivedType(typeof(Alternatives), "alt")]
[JsonDerivedType(typeof(Optional), "opt")]
[JsonDerivedType(typeof(Plural), "plural")]
[JsonDerivedType(typeof(Property), "prop")]
[JsonDerivedType(typeof(This), "this")]
public abstract record NibDefinition
{
    // Closed: these cases are the whole of the Nibs authoring surface.
    private NibDefinition()
    {
    }

    /// <summary>Literal text, matched exactly as written.</summary>
    public sealed record Literal(string Text) : NibDefinition;

    /// <summary>A regex, used as written - the explicit opt-in.</summary>
    public sealed record Pattern(string Regex) : NibDefinition;

    /// <summary>Exactly one of several literal texts.</summary>
    public sealed record Alternatives(IReadOnlyList<string> Texts) : NibDefinition;

    /// <summary>A nib that may be absent.</summary>
    public sealed record Optional(NibDefinition Inner) : NibDefinition;

    /// <summary>A nib that may be plural: the nib, then an optional plural suffix.</summary>
    public sealed record Plural(NibDefinition Inner) : NibDefinition;

    /// <summary>A reference to one of the glyph's own <see cref="GlyphDefinition.Properties"/>, by name.</summary>
    public sealed record Property(string Name) : NibDefinition;

    /// <summary>The document referring to itself: <see cref="Nib.This"/>.</summary>
    public sealed record This : NibDefinition;
}
