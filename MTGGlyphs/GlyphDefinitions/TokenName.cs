namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A token's name: "named wood", "named cloud sprite", "named wolves of the hunt" - the name a created token is given, which later text ("a token named wood") refers to.</summary>
/// <exampledoc>Jungle Patrol</exampledoc>
/// <examplecapture>named wood</examplecapture>
[Dependent]
public class TokenName : Glyph
{
    public override Nib[] Nibs => ["named", Pattern(@"[a-z][a-z'-]*(?: (?!(?:with|that's|that|at|for|and|is|equal)\b)[a-z][a-z'-]*)*")];
}
