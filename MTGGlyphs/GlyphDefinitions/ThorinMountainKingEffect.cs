namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thorin, Mountain-king, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thorin, Mountain-king</exampledoc>
/// <examplecapture>that creature deals damage equal to its power to up to one target creature</examplecapture>
[Dependent]
public class ThorinMountainKingEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature deals damage equal to its power to up to one target creature"];
}
