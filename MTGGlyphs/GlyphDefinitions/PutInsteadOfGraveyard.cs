namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A card going somewhere other than the graveyard it was headed to: "put it onto the battlefield instead of putting it into your graveyard", "… with two +1/+1 counters on it instead of putting it into your graveyard".</summary>
/// <exampledoc>Loxodon Smiter</exampledoc>
/// <examplecapture>put it onto the battlefield instead of putting it into your graveyard</examplecapture>
[Dependent]
public class PutInsteadOfGraveyard : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Put), "instead of putting it into your graveyard"];

    public PutIntoZone Put { get; set; }
}
