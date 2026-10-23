namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hazduhr the Abbot, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hazduhr the Abbot</exampledoc>
[Dependent]
public class HazduhrTheAbbotEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next x damage that would be dealt this turn to target white creature you control is dealt to hazduhr instead"];
}
