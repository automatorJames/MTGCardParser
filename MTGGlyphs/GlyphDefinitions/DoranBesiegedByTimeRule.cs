namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Doran, Besieged by Time, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Doran, Besieged by Time</exampledoc>
[TokenizationOrder(-1)]
public class DoranBesiegedByTimeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature spell you cast with toughness greater than its power costs {1} less to cast"];
}
