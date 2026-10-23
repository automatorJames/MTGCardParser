namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Anchor to Reality, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anchor to Reality</exampledoc>
[TokenizationOrder(-1)]
public class AnchorToRealityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it has mana value less than the sacrificed permanent's mana value, scry 2"];
}
