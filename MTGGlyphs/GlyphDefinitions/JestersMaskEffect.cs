namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jester's Mask, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jester's Mask</exampledoc>
[Dependent]
public class JestersMaskEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target opponent puts the cards from their hand on top of their library"];
}
