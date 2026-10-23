namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pyromancer Ascension, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pyromancer Ascension</exampledoc>
[TokenizationOrder(-1)]
public class PyromancerAscensionCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast an instant or sorcery spell that has the same name as a card in your graveyard, you may put a quest counter on this enchantment"];
}
