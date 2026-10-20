namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"if you attacked this turn", "if a creature died this turn": a trailing condition on something that happened this turn.</summary>
/// <exampledoc>Swaggering Corsair</exampledoc>
/// <examplecapture>if you attacked this turn</examplecapture>
[Dependent]
public class IfHappenedThisTurn : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Event)];

    public HappenedThisTurn Event { get; set; }
}
