namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jungle Patrol, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jungle Patrol</exampledoc>
[TokenizationOrder(-1)]
public class JunglePatrolRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice a token named wood: add {r}"];
}
