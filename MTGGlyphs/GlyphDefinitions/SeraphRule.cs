namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Seraph, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seraph</exampledoc>
[TokenizationOrder(-1)]
public class SeraphRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice the creature when you lose control of this creature"];
}
