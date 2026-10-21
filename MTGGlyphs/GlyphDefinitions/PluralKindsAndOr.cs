namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A list of permanent kinds joined by "and/or": "artifacts, legendaries, and/or sagas", "artifacts and/or creatures" - permanents that are any of them.</summary>
/// <exampledoc>Dáin, Lord of the Iron Hills</exampledoc>
/// <examplecapture>artifacts, legendaries, and/or sagas</examplecapture>
[Dependent]
public class PluralKindsAndOr : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(First), Pattern(",? and/or "), Prop(Last)];

    public CompoundOf<PluralKind> First { get; set; }
    public PluralKind Last { get; set; }
}
