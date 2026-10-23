namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Warbringer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Warbringer</exampledoc>
[TokenizationOrder(-1)]
public class WarbringerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["dash costs you pay cost {2} less (as long as this creature is on the battlefield)"];
}
