namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Interdict, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Interdict</exampledoc>
[TokenizationOrder(-1)]
public class Interdict2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that permanent's activated abilities can't be activated this turn"];
}
