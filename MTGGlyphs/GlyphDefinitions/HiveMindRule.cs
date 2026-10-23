namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Hive Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hive Mind</exampledoc>
[TokenizationOrder(-1)]
public class HiveMindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each of those players may choose new targets for their copy"];
}
