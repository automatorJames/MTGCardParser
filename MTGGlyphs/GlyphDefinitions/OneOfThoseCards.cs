namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Some cards out of a group named earlier, as something to play: "one of those cards", "one of them", "up to two of those cards", "any number of the copies", "up to three of the copies".</summary>
/// <exampledoc>Unlucky Witness</exampledoc>
/// <examplecapture>one of those cards</examplecapture>
[Dependent]
public class OneOfThoseCards : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), Prop(Count), "of", Prop(Cards)];

    [Optional]
    [RegexPattern("up to")]
    public bool? UpTo { get; set; }
    public Quantity Count { get; set; }
    public OneOf<ThatCardPhrase, Them, TheCopy> Cards { get; set; }
}
