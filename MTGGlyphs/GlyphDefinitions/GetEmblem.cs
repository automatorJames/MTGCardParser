namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A planeswalker's emblem: "you get an emblem with "…"", "target opponent gets an emblem with "…"". The quoted ability is any glyph that resolves; the closing quote falls after the sentence's period. Emblems with two abilities are <see cref="GetEmblemWithTwoAbilities"/>.</summary>
/// <exampledoc>Elspeth, Sun's Champion</exampledoc>
/// <examplecapture>you get an emblem with "creatures you control get +2/+2 and have flying</examplecapture>
public class GetEmblem : Glyph, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Pattern("gets?"), " an emblem with \"", Prop(Ability)];

    public DynamicGlyph Ability { get; set; }
}
