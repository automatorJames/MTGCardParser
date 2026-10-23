namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Call to Arms, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call to Arms</exampledoc>
/// <examplecapture>white creatures get +1/+1 as long as the chosen color is the most common color among nontoken permanents the chosen player controls but isn't tied for most common</examplecapture>
[TokenizationOrder(-1)]
public class CallToArmsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["white creatures get +1/+1 as long as the chosen color is the most common color among nontoken permanents the chosen player controls but isn't tied for most common"];
}
