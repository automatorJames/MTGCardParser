namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"at least two other creatures": a lower bound on a number of permanents of a kind, optionally excluding this one.</summary>
/// <exampledoc>Orcish Conscripts</exampledoc>
/// <examplecapture>at least two other creatures</examplecapture>
[Dependent]
public class AtLeastPermanents : Glyph
{
    public override Nib[] Nibs => ["at least", Prop(Count), Opt("other"), Prop(Kinds)];

    public Quantity Count { get; set; }
    public Permanents Kinds { get; set; }
}
