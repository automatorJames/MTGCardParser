namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Contested Game Ball, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Contested Game Ball</exampledoc>
[Dependent]
public class ContestedGameBallEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the attacking player gains control of this artifact and untaps it"];
}
