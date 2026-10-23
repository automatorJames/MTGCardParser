namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Temporal Extortion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temporal Extortion</exampledoc>
[Dependent]
public class TemporalExtortionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may pay half their life, rounded up"];
}
