namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Riftmarked Knight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riftmarked Knight</exampledoc>
[TokenizationOrder(-1)]
public class RiftmarkedKnightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when the last time counter is removed from this card while it's exiled, create a 2/2 black knight creature token with flanking, protection from white, and haste"];
}
