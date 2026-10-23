namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Quicksilver Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quicksilver Elemental</exampledoc>
[TokenizationOrder(-1)]
public class QuicksilverElementalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may spend blue mana as though it were mana of any color to pay the activation costs of this creature's abilities"];
}
