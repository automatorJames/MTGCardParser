namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Cards of a kind, named by qualifier words before "card": "creature card", "instant or sorcery cards". A separate alternative from bare "card(s)", since optional qualifiers directly before Plural("card") stopped the match when absent.</summary>
/// <exampledoc>Cold Storage</exampledoc>
/// <examplecapture>creature card</examplecapture>
[Dependent]
public class QualifiedCards : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Plural("card")];

    public QualifierWords Qualifiers { get; set; }
}
