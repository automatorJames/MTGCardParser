namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sheltered Valley, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sheltered Valley</exampledoc>
[TokenizationOrder(-1)]
public class ShelteredValleyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this land would enter, instead sacrifice each other permanent named", Nib.This, "you control, then put this land onto the battlefield"];
}
