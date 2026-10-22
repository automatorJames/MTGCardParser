namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you may spend mana as though it were mana of any color": the active form of the any-mana rider, by a player.</summary>
/// <exampledoc>Psychic Intrusion</exampledoc>
/// <examplecapture>you may spend mana as though it were mana of any color</examplecapture>
[Dependent]
public class MaySpendManaAsThough : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "may spend mana as though it were mana of any", Prop(Kind)];

    public OneOf<WhichPlayer?, They> Player { get; set; }
    public AnyManaKind Kind { get; set; }
}
