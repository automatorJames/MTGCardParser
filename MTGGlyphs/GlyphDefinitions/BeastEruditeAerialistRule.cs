namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Beast, Erudite Aerialist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beast, Erudite Aerialist</exampledoc>
[TokenizationOrder(-1)]
public class BeastEruditeAerialistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you've put one or more +1/+1 counters on beast this turn, he has flying"];
}
