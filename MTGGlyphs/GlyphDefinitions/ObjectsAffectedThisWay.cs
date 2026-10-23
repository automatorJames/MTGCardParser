namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a creature card put into your graveyard this way", "each card put into a graveyard this way": one, each or all of the objects the effect just described acted on, as something a later effect acts on.</summary>
/// <exampledoc>Starfall Invocation</exampledoc>
/// <examplecapture>a creature card put into your graveyard this way</examplecapture>
[Dependent]
public class ObjectsAffectedThisWay : Glyph
{
    public override Nib[] Nibs => [Prop(Quantifier), Prop(Affected)];

    public AffectedQuantifier Quantifier { get; set; }
    public AffectedThisWay Affected { get; set; }
}
