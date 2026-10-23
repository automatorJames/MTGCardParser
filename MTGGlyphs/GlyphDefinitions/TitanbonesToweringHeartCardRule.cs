namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Titanbones, Towering Heart, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Titanbones, Towering Heart</exampledoc>
[TokenizationOrder(-1)]
public class TitanbonesToweringHeartCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you discard this card, you gain 3 life"];
}
