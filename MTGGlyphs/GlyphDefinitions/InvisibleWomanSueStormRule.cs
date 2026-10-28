namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Invisible Woman, Sue Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Invisible Woman, Sue Storm</exampledoc>
[TokenizationOrder(-1)]
public class InvisibleWomanSueStormRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you put one or more +1/+1 counters on one or more other heroes you control, you may create a 0/4 colorless wall creature token with defender"];
}
