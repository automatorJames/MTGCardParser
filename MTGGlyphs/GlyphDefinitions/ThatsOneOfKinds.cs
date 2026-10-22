namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's a wolf or a werewolf", "that's a barbarian, a warrior, or a berserker": permanents narrowed to those of one of several kinds (tribal lords that name several types).</summary>
/// <exampledoc>Howlpack Resurgence</exampledoc>
/// <examplecapture>that's a wolf or a werewolf</examplecapture>
[Dependent]
public class ThatsOneOfKinds : Glyph
{
    public override Nib[] Nibs => ["that's", Prop(Kinds)];

    public ManyOf<ArticleKind> Kinds { get; set; }
}
