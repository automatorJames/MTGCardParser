namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Barbarian Class, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Barbarian Class</exampledoc>
[TokenizationOrder(-1)]
public class BarbarianClassRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would roll one or more dice, instead roll that many dice plus one and ignore the lowest roll"];
}
