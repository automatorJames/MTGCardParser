namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Radiant Kavu, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Radiant Kavu</exampledoc>
[Dependent]
public class RadiantKavuEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all combat damage blue creatures and black creatures would deal this turn"];
}
