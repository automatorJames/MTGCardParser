namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lo and Li, Twin Tutors, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lo and Li, Twin Tutors</exampledoc>
[TokenizationOrder(-1)]
public class LoAndLiTwinTutorsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when lo and li enter, search your library for a lesson or noble card, reveal it, put it into your hand, then shuffle"];
}
