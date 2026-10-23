namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ghostly Flame, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ghostly Flame</exampledoc>
[TokenizationOrder(-1)]
public class GhostlyFlameRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["black and/or red permanents and spells are colorless sources of damage"];
}
