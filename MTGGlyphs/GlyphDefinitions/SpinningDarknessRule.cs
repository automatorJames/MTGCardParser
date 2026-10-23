namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spinning Darkness, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinning Darkness</exampledoc>
[TokenizationOrder(-1)]
public class SpinningDarknessRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile the top three black cards of your graveyard rather than pay this spell's mana cost"];
}
