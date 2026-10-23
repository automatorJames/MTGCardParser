namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Evelyn, the Covetous, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evelyn, the Covetous</exampledoc>
[Dependent]
public class EvelynTheCovetous2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the top card of each player's library with a collection counter on it"];
}
