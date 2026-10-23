namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Titans' Nest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titans' Nest</exampledoc>
[TokenizationOrder(-1)]
public class TitansNestRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast a spell that's one or more colors without {x} in its mana cost"];
}
