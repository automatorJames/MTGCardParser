namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Desecrator Hag, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Desecrator Hag</exampledoc>
[Dependent]
public class DesecratorHagEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return to your hand the creature card in your graveyard with the greatest power"];
}
