namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Norn's Inquisitor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Norn's Inquisitor</exampledoc>
[TokenizationOrder(-1)]
public class NornsInquisitorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a permanent you control transforms into a phyrexian, put a +1/+1 counter on it"];
}
