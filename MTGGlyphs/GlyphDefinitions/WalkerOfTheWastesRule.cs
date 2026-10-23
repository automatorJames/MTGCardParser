namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Walker of the Wastes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Walker of the Wastes</exampledoc>
[TokenizationOrder(-1)]
public class WalkerOfTheWastesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature gets +1/+1 for each land you control named wastes"];
}
