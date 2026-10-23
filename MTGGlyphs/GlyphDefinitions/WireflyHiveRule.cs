namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wirefly Hive, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wirefly Hive</exampledoc>
[TokenizationOrder(-1)]
public class WireflyHiveRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, destroy all permanents named wirefly"];
}
