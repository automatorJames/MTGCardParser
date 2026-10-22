namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a creature card", "an enchantment card": one card of a kind.</summary>
/// <exampledoc>In the Presence of Ages</exampledoc>
/// <examplecapture>a land card</examplecapture>
[Dependent]
public class SingleCardOfKind : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Kind), "card"];

    public PermanentKind Kind { get; set; }
}
