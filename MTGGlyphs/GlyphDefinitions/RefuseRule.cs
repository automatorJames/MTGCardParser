namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Refuse // Cooperate, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Refuse // Cooperate</exampledoc>
[TokenizationOrder(-1)]
public class RefuseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["refuse deals damage to target spell's controller equal to that spell's mana value"];
}
