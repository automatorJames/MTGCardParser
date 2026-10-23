namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mirror March, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror March</exampledoc>
[Dependent]
public class MirrorMarchEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["flip a coin until you lose a flip"];
}
