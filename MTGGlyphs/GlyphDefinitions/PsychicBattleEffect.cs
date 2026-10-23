namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Psychic Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Battle</exampledoc>
[Dependent]
public class PsychicBattleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player reveals the top card of their library"];
}
