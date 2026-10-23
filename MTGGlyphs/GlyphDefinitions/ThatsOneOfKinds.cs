namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's a wolf or a werewolf", "that's a barbarian, a warrior, or a berserker", "that are fungi and/or saprolings": permanents narrowed to those of one of several kinds (tribal lords that name several types).</summary>
/// <exampledoc>Howlpack Resurgence</exampledoc>
/// <examplecapture>that's a wolf or a werewolf</examplecapture>
[Dependent]
public class ThatsOneOfKinds : Glyph
{
    public override Nib[] Nibs => [Alt("that's", "that are"), Prop(Kinds)];

    public OneOf<ManyOf<ArticleKind>, PluralKindsAndOr> Kinds { get; set; }
}
