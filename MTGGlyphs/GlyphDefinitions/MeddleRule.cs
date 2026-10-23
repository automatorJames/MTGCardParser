namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Meddle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Meddle</exampledoc>
[TokenizationOrder(-1)]
public class MeddleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if target spell has only one target and that target is a creature, change that spell's target to another creature"];
}
