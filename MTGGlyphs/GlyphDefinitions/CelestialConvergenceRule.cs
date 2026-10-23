namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Celestial Convergence, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Celestial Convergence</exampledoc>
[TokenizationOrder(-1)]
public class CelestialConvergenceRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more players are tied for highest life total, the game is a draw"];
}
