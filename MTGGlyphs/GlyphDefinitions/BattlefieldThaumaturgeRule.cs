namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Battlefield Thaumaturge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Battlefield Thaumaturge</exampledoc>
[TokenizationOrder(-1)]
public class BattlefieldThaumaturgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each instant and sorcery spell you cast costs {1} less to cast for each creature it targets"];
}
