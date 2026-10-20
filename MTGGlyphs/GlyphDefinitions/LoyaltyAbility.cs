namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A planeswalker's loyalty ability: a loyalty cost in brackets, a colon, and what it does - "[+1]: draw a card", "[−3]: destroy target creature". Activated like any ability, by adding or removing loyalty counters.</summary>
/// <exampledoc>Ob Nixilis Reignited</exampledoc>
/// <examplecapture>[−3]: destroy target creature</examplecapture>
public class LoyaltyAbility : Glyph
{
    public override Nib[] Nibs => [Prop(Cost), ":", Prop(Effect)];

    public LoyaltyCost Cost { get; set; }
    public DynamicGlyph Effect { get; set; }
}
