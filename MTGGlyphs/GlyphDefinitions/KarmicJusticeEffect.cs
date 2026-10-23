namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Karmic Justice, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Karmic Justice</exampledoc>
[Dependent]
public class KarmicJusticeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may destroy target permanent that opponent controls"];
}
