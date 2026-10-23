namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kid Loki, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kid Loki</exampledoc>
[TokenizationOrder(-1)]
public class KidLokiRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control that you've put one or more +1/+1 counters on this turn has hexproof"];
}
