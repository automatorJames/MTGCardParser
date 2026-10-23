namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teferi, Mage of Zhalfir, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi, Mage of Zhalfir</exampledoc>
[TokenizationOrder(-1)]
public class TeferiMageOfZhalfirRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creature cards you own that aren't on the battlefield have flash"];
}
