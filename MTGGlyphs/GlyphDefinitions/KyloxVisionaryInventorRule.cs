namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Kylox, Visionary Inventor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kylox, Visionary Inventor</exampledoc>
[TokenizationOrder(-1)]
public class KyloxVisionaryInventorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast any number of instant and/or sorcery spells from among the exiled cards without paying their mana costs"];
}
