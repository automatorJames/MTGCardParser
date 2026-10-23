namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Omenpath Journey, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Omenpath Journey</exampledoc>
[Dependent]
public class OmenpathJourneyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a card at random exiled with this enchantment and put it onto the battlefield tapped"];
}
