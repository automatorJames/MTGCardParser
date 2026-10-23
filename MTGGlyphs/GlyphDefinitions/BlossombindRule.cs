namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Blossombind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Blossombind</exampledoc>
[TokenizationOrder(-1)]
public class BlossombindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature can't become untapped and can't have counters put on it"];
}
