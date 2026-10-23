namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Broken Visage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Broken Visage</exampledoc>
/// <examplecapture>its power is equal to that creature's power and its toughness is equal to that creature's toughness</examplecapture>
[TokenizationOrder(-1)]
public class BrokenVisageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["its power is equal to that creature's power and its toughness is equal to that creature's toughness"];
}
