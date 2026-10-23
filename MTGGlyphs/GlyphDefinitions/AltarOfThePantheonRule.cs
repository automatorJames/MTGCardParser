namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Altar of the Pantheon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Altar of the Pantheon</exampledoc>
[TokenizationOrder(-1)]
public class AltarOfThePantheonRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your devotion to each color and each combination of colors is increased by one"];
}
