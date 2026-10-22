namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Each of a group named earlier in the line, receiving something one by one: "each of them", "each of those creatures", "each of those permanents".</summary>
/// <exampledoc>Nature's Panoply</exampledoc>
/// <examplecapture>each of them</examplecapture>
[Dependent]
public class EachOfThem : Glyph
{
    public override Nib[] Nibs => ["each of", Prop(Group)];

    public OneOf<Them, ThatCard, ThatCardPhrase> Group { get; set; }
}
