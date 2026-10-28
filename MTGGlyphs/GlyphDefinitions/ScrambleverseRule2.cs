namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scrambleverse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scrambleverse</exampledoc>
[TokenizationOrder(-1)]
public class ScrambleverseRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then each player gains control of each permanent for which they were chosen"];
}
