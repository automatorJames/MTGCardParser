namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Molten Exhale, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Molten Exhale</exampledoc>
[TokenizationOrder(-1)]
public class MoltenExhaleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast this spell as though it had flash if you behold a dragon as an additional cost to cast it"];
}
