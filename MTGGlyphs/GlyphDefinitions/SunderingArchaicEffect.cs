namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sundering Archaic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sundering Archaic</exampledoc>
/// <examplecapture>exile target nonland permanent an opponent controls with mana value less than or equal to the number of colors of mana spent to cast this creature</examplecapture>
[Dependent]
public class SunderingArchaicEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile target nonland permanent an opponent controls with mana value less than or equal to the number of colors of mana spent to cast this creature"];
}
