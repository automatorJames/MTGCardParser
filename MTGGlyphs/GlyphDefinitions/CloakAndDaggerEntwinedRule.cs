namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cloak and Dagger, Entwined, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cloak and Dagger, Entwined</exampledoc>
[TokenizationOrder(-1)]
public class CloakAndDaggerEntwinedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exile a nonland card from their hand or the chosen creature until cloak and dagger leave the battlefield"];
}
