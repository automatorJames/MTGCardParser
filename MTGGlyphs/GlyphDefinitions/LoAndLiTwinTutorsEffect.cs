namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lo and Li, Twin Tutors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lo and Li, Twin Tutors</exampledoc>
[Dependent]
public class LoAndLiTwinTutorsEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a lesson or noble card, reveal it, put it into your hand, then shuffle"];
}
