namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pollen Remedy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pollen Remedy</exampledoc>
[TokenizationOrder(-1)]
public class PollenRemedyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was kicked, prevent the next 6 damage this way instead"];
}
