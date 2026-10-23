namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Riftsweeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Riftsweeper</exampledoc>
[Dependent]
public class RiftsweeperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target face-up exiled card"];
}
