namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Druidic Satchel, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Druidic Satchel</exampledoc>
[TokenizationOrder(-1)]
public class DruidicSatchelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a noncreature, nonland card, you gain 2 life"];
}
