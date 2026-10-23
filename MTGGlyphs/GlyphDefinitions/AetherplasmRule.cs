namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aetherplasm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aetherplasm</exampledoc>
[TokenizationOrder(-1)]
public class AetherplasmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, you may put a creature card from your hand onto the battlefield blocking that creature"];
}
