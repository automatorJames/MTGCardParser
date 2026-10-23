namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bumi, King of Three Trials, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bumi, King of Three Trials</exampledoc>
[Dependent]
public class BumiKingOfThreeTrialsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose up to x, where x is the number of lesson cards in your graveyard —"];
}
