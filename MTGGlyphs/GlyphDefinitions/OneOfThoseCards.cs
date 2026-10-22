namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One card out of a group named earlier, as something to play: "one of those cards", "one of them".</summary>
/// <exampledoc>Unlucky Witness</exampledoc>
/// <examplecapture>one of those cards</examplecapture>
[Dependent]
public class OneOfThoseCards : Glyph
{
    public override Nib[] Nibs => ["one of", Prop(Cards)];

    public OneOf<ThatCardPhrase, Them> Cards { get; set; }
}
