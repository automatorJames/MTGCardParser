namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jace, Cunning Castaway, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, Cunning Castaway</exampledoc>
[Dependent]
public class JaceCunningCastawayEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create two tokens that are copies of jace, except they're not legendary"];
}
