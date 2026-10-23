namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dread Fugue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dread Fugue</exampledoc>
[TokenizationOrder(-1)]
public class DreadFugueRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you choose a nonland card from it [with mana value 2 or less]"];
}
