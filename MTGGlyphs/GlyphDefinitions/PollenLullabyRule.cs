namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pollen Lullaby, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pollen Lullaby</exampledoc>
[TokenizationOrder(-1)]
public class PollenLullabyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you win, creatures that player controls don't untap during the player's next untap step"];
}
