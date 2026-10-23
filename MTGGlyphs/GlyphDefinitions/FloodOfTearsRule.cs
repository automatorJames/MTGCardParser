namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Flood of Tears, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Flood of Tears</exampledoc>
[TokenizationOrder(-1)]
public class FloodOfTearsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you return four or more nontoken permanents you control this way, you may put a permanent card from your hand onto the battlefield"];
}
