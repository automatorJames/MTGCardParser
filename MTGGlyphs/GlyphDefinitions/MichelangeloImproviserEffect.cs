namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Michelangelo, Improviser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Michelangelo, Improviser</exampledoc>
[Dependent]
public class MichelangeloImproviserEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may put a creature card and/or a land card from your hand onto the battlefield"];
}
