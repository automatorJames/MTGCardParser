namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Call to the Kindred, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Call to the Kindred</exampledoc>
/// <examplecapture>if you do, you may put a creature card that shares a creature type with enchanted creature from among them onto the battlefield, then you put the rest of those cards on the bottom of your library in any order</examplecapture>
[TokenizationOrder(-1)]
public class CallToTheKindredRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you may put a creature card that shares a creature type with enchanted creature from among them onto the battlefield, then you put the rest of those cards on the bottom of your library in any order"];
}
