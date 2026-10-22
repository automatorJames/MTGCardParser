namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the creature tapped this way", "the card returned this way": one of the objects an effect just acted on, named with "the".</summary>
/// <exampledoc>Keldon Battlewagon</exampledoc>
/// <examplecapture>the creature tapped this way</examplecapture>
[Dependent]
public class TheAffectedThisWay : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Affected)];

    public AffectedThisWay Affected { get; set; }
}
