namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Goldspan Dragon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goldspan Dragon</exampledoc>
[TokenizationOrder(-1)]
public class GoldspanDragonCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature attacks or becomes the target of a spell, create a treasure token"];
}
