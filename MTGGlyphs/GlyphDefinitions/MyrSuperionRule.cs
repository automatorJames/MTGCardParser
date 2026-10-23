namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Myr Superion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Myr Superion</exampledoc>
[TokenizationOrder(-1)]
public class MyrSuperionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend only mana produced by creatures to cast this spell"];
}
