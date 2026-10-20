namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"creature tapped this way", "permanent sacrificed this way", "card revealed this way": each object the effect just described acted on.</summary>
/// <exampledoc>Ledev Champion</exampledoc>
/// <examplecapture>creature tapped this way</examplecapture>
[Dependent]
public class AffectedThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), Prop(Action), "this way"];

    public OneOf<PermanentKind, CardNoun?> Kind { get; set; }
    public ThisWayAction Action { get; set; }
}
