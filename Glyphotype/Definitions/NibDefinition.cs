namespace Glyphotype.Definitions;

/// <summary>
/// One entry of a <see cref="GlyphDefinition.Nibs"/> list - the portable counterpart of a <see cref="Nib"/>, and of
/// the <see cref="Glyph"/> helper that authors it: a plain string, <see cref="Glyph.Pattern"/>, <see cref="Glyph.Alt"/>,
/// <see cref="Glyph.Some"/>, <see cref="Glyph.Opt"/>, <see cref="Glyph.Plural"/> or <see cref="Glyph.Prop"/>. A property nib refers to its
/// property by name; the property itself is declared in <see cref="GlyphDefinition.Properties"/>.
/// The text helpers take literal text only, and don't nest; a property's treatments are its attributes (see <see cref="PropertyDefinition"/>).
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "nib")]
[JsonDerivedType(typeof(Literal), "literal")]
[JsonDerivedType(typeof(Pattern), "pattern")]
[JsonDerivedType(typeof(Alternatives), "alt")]
[JsonDerivedType(typeof(Some), "some")]
[JsonDerivedType(typeof(Optional), "opt")]
[JsonDerivedType(typeof(Plural), "plural")]
[JsonDerivedType(typeof(Property), "prop")]
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

    /// <summary>At least one of several literal texts, in order.</summary>
    public sealed record Some(IReadOnlyList<string> Texts) : NibDefinition;

    /// <summary>One of several literal texts, or none.</summary>
    public sealed record Optional(IReadOnlyList<string> Texts) : NibDefinition;

    /// <summary>A literal word or phrase, singular or plural.</summary>
    public sealed record Plural(string Text) : NibDefinition;

    /// <summary>A reference to one of the glyph's own <see cref="GlyphDefinition.Properties"/>, by name.</summary>
    public sealed record Property(string Name) : NibDefinition;
}
