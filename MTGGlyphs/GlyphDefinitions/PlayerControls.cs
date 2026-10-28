namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you control three or more artifacts", "an opponent controls a nontoken white permanent", "you control a swamp or a mountain", "you control at least five other mountains", "you control the creature with the greatest power or tied for the greatest power": a player controlling permanents of a kind, a choice of kinds, a number of them, or the most extreme one, as a condition.</summary>
/// <exampledoc>Auriok Sunchaser</exampledoc>
/// <examplecapture>you control three or more artifacts</examplecapture>
[Dependent]
public class PlayerControls : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public WhichPlayer Controller { get; set; }
    public OneOf<PermanentsThreshold, AtLeastPermanents, ManyOf<IndefinitePermanent>, ExtremePermanent, IndefinitePermanent> Permanent { get; set; }
}
