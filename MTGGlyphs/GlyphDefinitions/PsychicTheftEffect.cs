namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Psychic Theft, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Theft</exampledoc>
[Dependent]
public class PsychicTheftEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you haven't cast the card, return it to its owner's hand"];
}
