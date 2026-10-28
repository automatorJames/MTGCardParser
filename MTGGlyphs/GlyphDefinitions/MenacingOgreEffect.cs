namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Menacing Ogre, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Menacing Ogre</exampledoc>
[Dependent]
public class MenacingOgreEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player secretly chooses a number"];
}
