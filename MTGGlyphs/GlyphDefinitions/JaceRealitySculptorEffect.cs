namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jace, Reality Sculptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace, Reality Sculptor</exampledoc>
[Dependent]
public class JaceRealitySculptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all but the bottom card of each opponent's library"];
}
