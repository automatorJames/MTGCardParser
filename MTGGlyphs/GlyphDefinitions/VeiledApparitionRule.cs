namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Veiled Apparition, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Veiled Apparition</exampledoc>
[TokenizationOrder(-1)]
public class VeiledApparitionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when an opponent casts a spell, if this permanent is an enchantment, it becomes a 3/3 illusion creature with flying and \"at the beginning of your upkeep, sacrifice this creature unless you pay {1}{u}"];
}
