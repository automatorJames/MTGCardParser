namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Menacing Ogre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Menacing Ogre</exampledoc>
[TokenizationOrder(-1)]
public class MenacingOgreRule3 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you are one of those players, put two +1/+1 counters on this creature"];
}
