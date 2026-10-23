namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Null Chamber, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Null Chamber</exampledoc>
[TokenizationOrder(-1)]
public class NullChamberRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells with the chosen names can't be cast and lands with the chosen names can't be played"];
}
