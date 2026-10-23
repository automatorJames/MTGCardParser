namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Torment of Hailfire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Torment of Hailfire</exampledoc>
[TokenizationOrder(-1)]
public class TormentOfHailfireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["repeat the following process x times"];
}
