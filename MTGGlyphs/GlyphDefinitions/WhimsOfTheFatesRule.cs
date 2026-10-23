namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Whims of the Fates, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Whims of the Fates</exampledoc>
[TokenizationOrder(-1)]
public class WhimsOfTheFatesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player chooses one of their piles at random and sacrifices those permanents"];
}
