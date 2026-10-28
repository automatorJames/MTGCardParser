namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bloodfeather Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bloodfeather Phoenix</exampledoc>
[TokenizationOrder(-1)]
public class BloodfeatherPhoenixRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever an instant or sorcery spell you control deals damage to an opponent or battle, you may pay {r}"];
}
