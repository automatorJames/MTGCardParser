namespace Glyphotype.NibHelpers;

/// <summary>
/// A nib that may be plural (see <see cref="GlyphPrimitives.Glyph.Plural"/>): <c>Plural("card")</c> matches "card" and
/// "cards", <c>Plural(Prop(CardType))</c> "creature" and "creatures". The engine matches it as <see cref="Inner"/>
/// followed by an <see cref="OptionalPluralNib"/> suffix.
/// </summary>
public record PluralNib : Nib
{
    /// <summary>The nib made plural.</summary>
    public Nib Inner { get; }

    public PluralNib(Nib inner)
        : base(inner?.Text, inner?.Regex)
    {
        Inner = inner is null ? throw new ArgumentNullException(nameof(inner))
            : NibNesting.GetPluralError(NibNesting.KindOf(inner), inner.Text) is string error ? throw new ArgumentException(error, nameof(inner))
            : inner;
    }

    /// <summary><paramref name="nibs"/> as the engine matches them: each <see cref="PluralNib"/> as its inner nib, then an <see cref="OptionalPluralNib"/>.</summary>
    public static Nib[] Flatten(IEnumerable<Nib> nibs) =>
        nibs.SelectMany(x => x is PluralNib plural ? [plural.Inner, new OptionalPluralNib()] : new[] { x }).ToArray();
}
