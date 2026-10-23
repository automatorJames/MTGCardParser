namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Riftmarked Knight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riftmarked Knight</exampledoc>
/// <examplecapture>create a 2/2 black knight creature token with flanking, protection from white, and haste</examplecapture>
[Dependent]
public class RiftmarkedKnightEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a 2/2 black knight creature token with flanking, protection from white, and haste"];
}
