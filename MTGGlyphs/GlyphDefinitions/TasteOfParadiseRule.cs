namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Taste of Paradise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taste of Paradise</exampledoc>
[TokenizationOrder(-1)]
public class TasteOfParadiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain 3 life plus an additional 3 life for each additional {1}{g} you paid"];
}
