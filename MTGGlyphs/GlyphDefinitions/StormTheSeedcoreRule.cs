namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Storm the Seedcore, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Storm the Seedcore</exampledoc>
[TokenizationOrder(-1)]
public class StormTheSeedcoreRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["distribute four +1/+1 counters among up to four target creatures you control"];
}
