namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Runesword, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runesword</exampledoc>
[TokenizationOrder(-1)]
public class RuneswordRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the creature deals damage to a creature this turn, the creature dealt damage can't be regenerated this turn"];
}
