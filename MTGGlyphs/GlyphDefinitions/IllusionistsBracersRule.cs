namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Illusionist's Bracers, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Illusionist's Bracers</exampledoc>
[TokenizationOrder(-1)]
public class IllusionistsBracersRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an ability of equipped creature is activated, if it isn't a mana ability, copy that ability"];
}
