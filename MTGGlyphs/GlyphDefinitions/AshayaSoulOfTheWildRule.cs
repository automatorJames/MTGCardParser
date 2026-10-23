namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ashaya, Soul of the Wild, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ashaya, Soul of the Wild</exampledoc>
[TokenizationOrder(-1)]
public class AshayaSoulOfTheWildRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["nontoken creatures you control are forest lands in addition to their other types"];
}
