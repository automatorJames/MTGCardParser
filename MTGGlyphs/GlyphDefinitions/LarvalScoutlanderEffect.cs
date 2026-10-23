namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Larval Scoutlander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Larval Scoutlander</exampledoc>
[Dependent]
public class LarvalScoutlanderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may sacrifice a land or lander"];
}
