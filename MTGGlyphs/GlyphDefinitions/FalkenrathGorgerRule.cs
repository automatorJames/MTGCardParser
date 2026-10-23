namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Falkenrath Gorger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falkenrath Gorger</exampledoc>
[TokenizationOrder(-1)]
public class FalkenrathGorgerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each vampire creature card you own that isn't on the battlefield has madness"];
}
