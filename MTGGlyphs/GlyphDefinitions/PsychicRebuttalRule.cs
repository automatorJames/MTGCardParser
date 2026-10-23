namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Psychic Rebuttal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Rebuttal</exampledoc>
[TokenizationOrder(-1)]
public class PsychicRebuttalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spell mastery — if there are two or more instant and/or sorcery cards in your graveyard, you may copy the spell countered this way"];
}
