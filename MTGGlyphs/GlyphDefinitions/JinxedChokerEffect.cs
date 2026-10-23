namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jinxed Choker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jinxed Choker</exampledoc>
[Dependent]
public class JinxedChokerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a charge counter on this artifact or remove one from it"];
}
