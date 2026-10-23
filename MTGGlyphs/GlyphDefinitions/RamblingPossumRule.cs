namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rambling Possum, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rambling Possum</exampledoc>
[TokenizationOrder(-1)]
public class RamblingPossumRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then you may return any number of creatures that saddled it this turn to their owner's hand"];
}
