namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sword of the Meek, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sword of the Meek</exampledoc>
[TokenizationOrder(-1)]
public class SwordOfTheMeekRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a 1/1 creature you control enters, you may return this card from your graveyard to the battlefield, then attach it to that creature"];
}
