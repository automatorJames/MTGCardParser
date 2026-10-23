namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Thousand-Year Storm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thousand-Year Storm</exampledoc>
[Dependent]
public class ThousandYearStormEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy it for each other instant and sorcery spell you've cast before it this turn"];
}
