namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Thran Portal, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Thran Portal</exampledoc>
[TokenizationOrder(-1)]
public class ThranPortalRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["mana abilities of this land cost an additional 1 life to activate"];
}
