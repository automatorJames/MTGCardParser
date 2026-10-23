namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Psychic Surgery, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Psychic Surgery</exampledoc>
[Dependent]
public class PsychicSurgeryEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may look at the top two cards of that library"];
}
