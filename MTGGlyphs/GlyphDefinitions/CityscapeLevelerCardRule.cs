namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cityscape Leveler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cityscape Leveler</exampledoc>
[TokenizationOrder(-1)]
public class CityscapeLevelerCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you cast this spell and whenever this creature attacks, destroy up to one target nonland permanent"];
}
