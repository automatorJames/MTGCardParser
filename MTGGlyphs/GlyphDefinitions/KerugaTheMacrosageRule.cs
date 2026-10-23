namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Keruga, the Macrosage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keruga, the Macrosage</exampledoc>
[TokenizationOrder(-1)]
public class KerugaTheMacrosageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — your starting deck contains only cards with mana value 3 or greater and land cards"];
}
