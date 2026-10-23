namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cover of Winter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cover of Winter</exampledoc>
/// <examplecapture>if a creature would deal combat damage to you and/or one or more creatures you control, prevent x of that damage, where x is the number of age counters on this enchantment</examplecapture>
[TokenizationOrder(-1)]
public class CoverOfWinterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature would deal combat damage to you and/or one or more creatures you control, prevent x of that damage, where x is the number of age counters on this enchantment"];
}
