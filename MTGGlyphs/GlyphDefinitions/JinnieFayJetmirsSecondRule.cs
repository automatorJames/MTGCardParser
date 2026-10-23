namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Jinnie Fay, Jetmir's Second, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jinnie Fay, Jetmir's Second</exampledoc>
/// <examplecapture>if you would create one or more tokens, you may instead create that many 2/2 green cat creature tokens with haste or that many 3/1 green dog creature tokens with vigilance</examplecapture>
[TokenizationOrder(-1)]
public class JinnieFayJetmirsSecondRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you would create one or more tokens, you may instead create that many 2/2 green cat creature tokens with haste or that many 3/1 green dog creature tokens with vigilance"];
}
