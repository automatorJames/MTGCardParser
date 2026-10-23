namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aspect of Wolf, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aspect of Wolf</exampledoc>
/// <examplecapture>enchanted creature gets +x/+y, where x is half the number of forests you control, rounded down, and y is half the number of forests you control, rounded up</examplecapture>
[TokenizationOrder(-1)]
public class AspectOfWolfRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature gets +x/+y, where x is half the number of forests you control, rounded down, and y is half the number of forests you control, rounded up"];
}
