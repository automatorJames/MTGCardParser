namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vizier of the Menagerie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vizier of the Menagerie</exampledoc>
[TokenizationOrder(-1)]
public class VizierOfTheMenagerieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can spend mana of any type to cast creature spells"];
}
