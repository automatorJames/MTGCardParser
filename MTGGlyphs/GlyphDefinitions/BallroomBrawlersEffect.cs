namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Ballroom Brawlers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ballroom Brawlers</exampledoc>
/// <examplecapture>this creature and up to one other target creature you control both gain your choice of first strike or lifelink until end of turn</examplecapture>
[Dependent]
public class BallroomBrawlersEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature and up to one other target creature you control both gain your choice of first strike or lifelink until end of turn"];
}
