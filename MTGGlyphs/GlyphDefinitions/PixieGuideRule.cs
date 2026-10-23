namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pixie Guide, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pixie Guide</exampledoc>
[TokenizationOrder(-1)]
public class PixieGuideRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["grant an advantage — if you would roll one or more dice, instead roll that many dice plus one and ignore the lowest roll"];
}
