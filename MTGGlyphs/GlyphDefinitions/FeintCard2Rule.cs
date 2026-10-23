namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Feint, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Feint</exampledoc>
[TokenizationOrder(-1)]
public class FeintCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["tap all creatures blocking target attacking creature"];
}
