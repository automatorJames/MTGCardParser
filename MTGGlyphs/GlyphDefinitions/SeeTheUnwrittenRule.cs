namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of See the Unwritten, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>See the Unwritten</exampledoc>
[TokenizationOrder(-1)]
public class SeeTheUnwrittenRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ferocious — if you control a creature with power 4 or greater, you may put two creature cards onto the battlefield instead of one"];
}
