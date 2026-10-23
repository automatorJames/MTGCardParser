namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Melding when both halves are there: "if you both own and control this creature and a creature named midnight scavengers, exile them, then meld them into chittering host". The condition and the meld are one fixed procedure, so they're matched together.</summary>
/// <exampledoc>Graf Rats // Chittering Host</exampledoc>
/// <examplecapture>if you both own and control this creature and a creature named midnight scavengers, exile them, then meld them into chittering host</examplecapture>
public class MeldIfBoth : Glyph, IPredicate
{
    public override Nib[] Nibs => ["if", Prop(Condition), ",", Prop(Meld)];

    public OwnAndControlBoth Condition { get; set; }
    public MeldInto Meld { get; set; }
}
