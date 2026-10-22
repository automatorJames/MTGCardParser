namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A named optional action not taken: "don't cast it", "doesn't discard a card this way", "don't put it onto the battlefield" - the condition of a fallback effect.</summary>
/// <exampledoc>Descendants' Path</exampledoc>
/// <examplecapture>don't cast it</examplecapture>
[Dependent]
public class DidNotDo : Glyph
{
    public override Nib[] Nibs => [Alt("don't", "doesn't"), Prop(Action), Prop(ThisWay)];

    public OneOf<PlayCard, PutIntoZone, DrawOrDiscardCards> Action { get; set; }
    [Optional]
    [RegexPattern("this way")]
    public bool? ThisWay { get; set; }
}
