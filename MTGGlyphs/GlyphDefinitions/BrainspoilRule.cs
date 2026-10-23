namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Brainspoil, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Brainspoil</exampledoc>
[TokenizationOrder(-1)]
public class BrainspoilRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target creature that isn't enchanted"];
}
