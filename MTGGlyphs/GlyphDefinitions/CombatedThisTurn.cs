namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent having attacked or blocked this turn: "this creature attacked this turn", "it blocked this turn", "this creature attacked or blocked this turn", or met a particular creature in combat: "this creature blocked or was blocked by a blue creature this turn".</summary>
/// <exampledoc>Bloodsoaked Champion</exampledoc>
/// <examplecapture>this creature attacked this turn</examplecapture>
[Dependent]
public class CombatedThisTurn : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), Prop(Action), Prop(Partner), "this turn"];

    public PermanentPhrase Permanent { get; set; }
    public CombatHistory Action { get; set; }
    [Optional]
    public CombatPartner Partner { get; set; }
}
