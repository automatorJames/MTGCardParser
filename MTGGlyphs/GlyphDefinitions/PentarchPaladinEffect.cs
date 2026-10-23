namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Pentarch Paladin, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pentarch Paladin</exampledoc>
[Dependent]
public class PentarchPaladinEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target permanent of the chosen color"];
}
