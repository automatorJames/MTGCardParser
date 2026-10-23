namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The damage something was dealt this turn, as an amount: "the damage dealt to your opponents this turn", "the damage dealt to you this turn", "the damage dealt to target opponent this turn", "the amount of damage dealt to it this turn".</summary>
/// <exampledoc>Notorious Throng</exampledoc>
/// <examplecapture>the damage dealt to your opponents this turn</examplecapture>
[Dependent]
public class DamageDealtThisTurn : Glyph
{
    public override Nib[] Nibs => ["the", Opt("amount of"), "damage dealt to", Prop(Recipient), "this turn"];

    public OneOf<WhichPlayer?, It, SpecificTarget> Recipient { get; set; }
}
