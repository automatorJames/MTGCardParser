namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hellkite Hatchling, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hellkite Hatchling</exampledoc>
[TokenizationOrder(-1)]
public class HellkiteHatchlingRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature has flying and trample if it devoured a creature"];
}
