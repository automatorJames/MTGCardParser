namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent picked out by having been dealt damage: "a creature dealt damage this way", "a creature dealt damage by this creature this turn".</summary>
/// <exampledoc>Annihilating Fire</exampledoc>
/// <examplecapture>a creature dealt damage this way</examplecapture>
[Dependent]
public class DamagedPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "dealt damage", Prop(Source)];

    public IndefinitePermanent Permanent { get; set; }
    public OneOf<ThisWay, DamageDealtBy> Source { get; set; }
}
