namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mishra's Domination, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mishra's Domination</exampledoc>
[TokenizationOrder(-1)]
public class MishrasDominationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as long as you control enchanted creature, it gets +2/+2"];
}
