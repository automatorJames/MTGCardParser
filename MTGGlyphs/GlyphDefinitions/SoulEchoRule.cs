namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Echo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Echo</exampledoc>
/// <examplecapture>otherwise, target opponent may choose that for each 1 damage that would be dealt to you until your next upkeep, you remove an echo counter from this enchantment instead</examplecapture>
[TokenizationOrder(-1)]
public class SoulEchoRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["otherwise, target opponent may choose that for each 1 damage that would be dealt to you until your next upkeep, you remove an echo counter from this enchantment instead"];
}
