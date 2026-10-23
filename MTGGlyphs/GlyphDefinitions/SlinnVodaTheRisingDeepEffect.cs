namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Slinn Voda, the Rising Deep, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Slinn Voda, the Rising Deep</exampledoc>
[Dependent]
public class SlinnVodaTheRisingDeepEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it was kicked, return all creatures to their owners' hands except for merfolk, krakens, leviathans, octopuses, and serpents"];
}
