namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cityscape Leveler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cityscape Leveler</exampledoc>
[Dependent]
public class CityscapeLevelerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy up to one target nonland permanent"];
}
