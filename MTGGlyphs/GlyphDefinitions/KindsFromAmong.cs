namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A list of permanent kinds, one of each to be chosen out of a group of permanents: "an artifact, a creature, an enchantment, and a planeswalker from among the nonland permanents they control" - as each player keeps one of each before sacrificing the rest.</summary>
/// <exampledoc>Cataclysmic Gearhulk</exampledoc>
/// <examplecapture>an artifact, a creature, an enchantment, and a planeswalker from among the nonland permanents they control</examplecapture>
[Dependent]
public class KindsFromAmong : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), "from among the", Prop(Pool)];

    public ManyOf<ArticleKind> Kinds { get; set; }
    public Permanents Pool { get; set; }
}
