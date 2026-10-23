namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Narset Transcendent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Narset Transcendent</exampledoc>
[TokenizationOrder(-1)]
public class NarsetTranscendentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it's a noncreature, nonland card, you may reveal it and put it into your hand"];
}
