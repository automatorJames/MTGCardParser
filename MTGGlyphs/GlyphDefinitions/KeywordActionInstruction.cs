namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action as a spell's whole instruction: "detain target creature an opponent controls", "suspect up to one target creature", "airbend target nonland permanent", "manifest the top card of your library". Tried after every other glyph, so verbs with their own glyphs keep them; nested uses take <see cref="KeywordActionOn"/> directly.</summary>
/// <exampledoc>Airbending Lesson</exampledoc>
/// <examplecapture>airbend target nonland permanent</examplecapture>
[TokenizationOrder(-1)]
public class KeywordActionInstruction : Glyph
{
    public override Nib[] Nibs => [Prop(Action)];

    public KeywordActionOn Action { get; set; }
}
