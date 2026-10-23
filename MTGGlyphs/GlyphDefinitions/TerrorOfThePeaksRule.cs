namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Terror of the Peaks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Terror of the Peaks</exampledoc>
[TokenizationOrder(-1)]
public class TerrorOfThePeaksRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells your opponents cast that target this creature cost an additional 3 life to cast"];
}
