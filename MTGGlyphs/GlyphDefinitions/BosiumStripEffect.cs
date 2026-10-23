namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bösium Strip, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bösium Strip</exampledoc>
[Dependent]
public class BosiumStripEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, you may cast instant and sorcery spells from the top of your graveyard"];
}
