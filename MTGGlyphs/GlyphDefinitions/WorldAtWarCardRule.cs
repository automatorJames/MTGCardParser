namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of World at War, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>World at War</exampledoc>
[TokenizationOrder(-1)]
public class WorldAtWarCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of that combat, untap all creatures that attacked this turn"];
}
