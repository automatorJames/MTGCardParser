namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Manifest dread's choice, as a predicate in either person: "puts one onto the battlefield face down as a 2/2 creature and the other into their graveyard" (the third-person reminder, after "that player looks at the top two cards of their library, then").</summary>
/// <exampledoc>Fear of Impostors</exampledoc>
/// <examplecapture>puts one onto the battlefield face down as a 2/2 creature and the other into their graveyard</examplecapture>
[Dependent]
public class ManifestOneFaceDown : Glyph, IPredicate
{
    public override Nib[] Nibs => [Pattern("puts?"), "one onto the battlefield face down as a", Prop(Stats), "creature and the other into", Prop(Whose), "graveyard"];

    public PowerToughness Stats { get; set; }
    public Whose Whose { get; set; }
}
