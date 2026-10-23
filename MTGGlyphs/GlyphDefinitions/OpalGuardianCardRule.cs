namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Opal Guardian, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Opal Guardian</exampledoc>
[TokenizationOrder(-1)]
public class OpalGuardianCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when an opponent casts a creature spell, if this permanent is an enchantment, this enchantment becomes a 3/4 gargoyle creature with flying and protection from red"];
}
