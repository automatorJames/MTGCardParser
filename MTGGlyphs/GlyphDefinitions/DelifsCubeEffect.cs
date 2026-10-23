namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Delif's Cube, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Delif's Cube</exampledoc>
/// <examplecapture>this turn, when target creature you control attacks and isn't blocked, it assigns no combat damage this turn and you put a cube counter on this artifact</examplecapture>
[Dependent]
public class DelifsCubeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this turn, when target creature you control attacks and isn't blocked, it assigns no combat damage this turn and you put a cube counter on this artifact"];
}
