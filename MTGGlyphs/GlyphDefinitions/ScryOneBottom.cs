namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Scry 1's reminder text, second sentence: "you may put that card on the bottom of your library". Redundant with SubjectDoes → MayDo → PutIntoZone; kept (tried first) until its removal is approved.</summary>
/// <exampledoc>Bolt of Keranos</exampledoc>
/// <examplecapture>you may put that card on the bottom of your library</examplecapture>
[TokenizationOrder(0)]
public class ScryOneBottom : Glyph
{
    public override Nib[] Nibs => ["you may put that card on the bottom of your library"];
}
