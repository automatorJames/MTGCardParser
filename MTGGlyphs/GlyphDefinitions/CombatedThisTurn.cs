namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having attacked or blocked this turn: "this creature attacked this turn", "it blocked this turn", "this creature attacked or blocked this turn".</summary>
/// <exampledoc>Bloodsoaked Champion</exampledoc>
/// <examplecapture>this creature attacked this turn</examplecapture>
[Dependent]
public class CombatedThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), Prop(Action), "this turn"];

    public PermanentPhrase Permanent { get; set; }
    public CombatHistory Action { get; set; }
}
