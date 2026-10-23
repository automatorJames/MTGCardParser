namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Longstalk Brawl, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Longstalk Brawl</exampledoc>
[TokenizationOrder(-1)]
public class LongstalkBrawlRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter on the creature you control if the gift was promised"];
}
