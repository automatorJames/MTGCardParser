namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Battle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Battle</exampledoc>
[TokenizationOrder(-1)]
public class PsychicBattleCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a player chooses one or more targets, each player reveals the top card of their library"];
}
