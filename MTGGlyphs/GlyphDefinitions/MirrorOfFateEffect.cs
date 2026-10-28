namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mirror of Fate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror of Fate</exampledoc>
[Dependent]
public class MirrorOfFateEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose up to seven face-up exiled cards you own"];
}
