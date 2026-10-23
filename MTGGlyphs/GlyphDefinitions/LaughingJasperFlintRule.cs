namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Laughing Jasper Flint, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Laughing Jasper Flint</exampledoc>
[TokenizationOrder(-1)]
public class LaughingJasperFlintRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures you control but don't own are mercenaries in addition to their other types"];
}
