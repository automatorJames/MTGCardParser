namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Graceful Takedown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Graceful Takedown</exampledoc>
/// <examplecapture>any number of target enchanted creatures you control and up to one other target creature you control each deal damage equal to their power to target creature you don't control</examplecapture>
[TokenizationOrder(-1)]
public class GracefulTakedownRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any number of target enchanted creatures you control and up to one other target creature you control each deal damage equal to their power to target creature you don't control"];
}
