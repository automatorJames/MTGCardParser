namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Disrupting Shoal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Disrupting Shoal</exampledoc>
[TokenizationOrder(-1)]
public class DisruptingShoalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell if its mana value is x"];
}
