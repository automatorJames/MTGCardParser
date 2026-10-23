namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Squee, the Immortal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Squee, the Immortal</exampledoc>
[TokenizationOrder(-1)]
public class SqueeTheImmortalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this card from your graveyard or from exile"];
}
