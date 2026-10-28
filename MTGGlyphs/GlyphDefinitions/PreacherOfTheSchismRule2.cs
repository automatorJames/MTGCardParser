namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Preacher of the Schism, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Preacher of the Schism</exampledoc>
[TokenizationOrder(-1)]
public class PreacherOfTheSchismRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature attacks while you have the most life or are tied for most life, you draw a card and you lose 1 life"];
}
