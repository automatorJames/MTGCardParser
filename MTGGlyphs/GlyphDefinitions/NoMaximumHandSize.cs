namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"has no maximum hand size": a player exempt from discarding down to seven at cleanup.</summary>
/// <exampledoc>Graceful Adept</exampledoc>
/// <examplecapture>have no maximum hand size</examplecapture>
[Dependent]
public class NoMaximumHandSize : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("has|have"), "no maximum hand size"];
}
