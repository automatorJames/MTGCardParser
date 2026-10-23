namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wretched Banquet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wretched Banquet</exampledoc>
[TokenizationOrder(-1)]
public class WretchedBanquetRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target creature if it has the least power or is tied for least power among creatures on the battlefield"];
}
