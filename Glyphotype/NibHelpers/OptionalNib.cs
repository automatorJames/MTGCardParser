namespace Glyphotype.NibHelpers;

/// <summary>One of several literal texts, or none (see <see cref="GlyphPrimitives.Glyph.Opt"/>).</summary>
public record OptionalNib : Nib
{
    /// <summary>The texts as originally passed to <see cref="GlyphPrimitives.Glyph.Opt"/>, so a display-only consumer can reconstruct the original <c>Opt(...)</c> call.</summary>
    public string[] Texts { get; }

    public OptionalNib(params string[] texts)
        : base(string.Join('|', RequireTexts("Opt", texts, minimum: 1)), texts.Length == 1 ? EscapeLiteral(texts[0]) : "(" + string.Join('|', texts.Select(EscapeLiteral)) + ")")
    {
        Texts = texts;
    }

    public override bool IsOptional => true;

    public override string Authored => $"Opt({Quote(Texts)})";

    public override IReadOnlyList<string> Literals => Texts;

    /// <summary>Each text, which may open differently (see <see cref="NibAlternatives.Branches"/>).</summary>
    public override IReadOnlyList<string> Branches(Joiner joiner) =>
        Texts.Select(EscapeLiteral).ToList();
}
