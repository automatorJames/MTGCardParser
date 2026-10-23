namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Osteomancer Adept, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Osteomancer Adept</exampledoc>
[Dependent]
public class OsteomancerAdeptEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, you may cast creature spells from your graveyard by foraging in addition to paying their other costs"];
}
