namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Opal Titan, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Opal Titan</exampledoc>
[TokenizationOrder(-1)]
public class OpalTitanCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when an opponent casts a creature spell, if this permanent is an enchantment, it becomes a 4/4 giant creature with protection from each of that spell's colors"];
}
