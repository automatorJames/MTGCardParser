namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Coveted Falcon, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Coveted Falcon</exampledoc>
[Dependent]
public class CovetedFalconEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["gain control of target permanent you own but don't control"];
}
