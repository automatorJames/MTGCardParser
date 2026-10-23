namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Choreographed Sparks, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Choreographed Sparks</exampledoc>
[TokenizationOrder(-1)]
public class ChoreographedSparksRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy gains haste and \"at the beginning of the end step, sacrifice this token"];
}
