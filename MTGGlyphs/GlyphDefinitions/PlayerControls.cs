namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you control three or more artifacts", "an opponent controls a nontoken white permanent", "you control a swamp or a mountain", "you control at least five other mountains": a player controlling permanents of a kind, a choice of kinds, or a number of them, as a condition.</summary>
/// <exampledoc>Auriok Sunchaser</exampledoc>
/// <examplecapture>you control three or more artifacts</examplecapture>
[Dependent]
public class PlayerControls : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public WhichPlayer Controller { get; set; }
    public OneOf<OneOf<PermanentsThreshold, AtLeastPermanents>, ManyOf<IndefinitePermanent>, IndefinitePermanent> Permanent { get; set; }
}
