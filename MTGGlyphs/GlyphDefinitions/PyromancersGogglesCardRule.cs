namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pyromancer's Goggles, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyromancer's Goggles</exampledoc>
[TokenizationOrder(-1)]
public class PyromancersGogglesCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when that mana is spent to cast a red instant or sorcery spell, copy that spell and you may choose new targets for the copy"];
}
