namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Psychic Possession, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Possession</exampledoc>
[Dependent]
public class PsychicPossessionEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may draw a card"];
}
