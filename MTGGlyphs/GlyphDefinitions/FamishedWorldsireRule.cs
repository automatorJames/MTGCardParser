namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Famished Worldsire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Famished Worldsire</exampledoc>
[TokenizationOrder(-1)]
public class FamishedWorldsireRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["devour land 3 (as this creature enters, you may sacrifice any number of lands"];
}
