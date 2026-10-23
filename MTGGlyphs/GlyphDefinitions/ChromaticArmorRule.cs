namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chromatic Armor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chromatic Armor</exampledoc>
[TokenizationOrder(-1)]
public class ChromaticArmorRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt to enchanted creature by sources of the last chosen color"];
}
