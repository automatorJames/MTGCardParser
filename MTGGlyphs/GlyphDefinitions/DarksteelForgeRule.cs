namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Darksteel Forge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darksteel Forge</exampledoc>
[TokenizationOrder(-1)]
public class DarksteelForgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["artifact creatures with indestructible can't be destroyed by damage"];
}
