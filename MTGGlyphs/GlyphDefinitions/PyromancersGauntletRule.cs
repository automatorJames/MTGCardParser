namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pyromancer's Gauntlet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyromancer's Gauntlet</exampledoc>
/// <examplecapture>if a red instant or sorcery spell you control or a red planeswalker you control would deal damage to a permanent or player, it deals that much damage plus 2 to that permanent or player instead</examplecapture>
[TokenizationOrder(-1)]
public class PyromancersGauntletRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a red instant or sorcery spell you control or a red planeswalker you control would deal damage to a permanent or player, it deals that much damage plus 2 to that permanent or player instead"];
}
