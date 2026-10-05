namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class GainedOrLostBuff : Glyph
{
    public override Nib[] Nibs => [Prop(PermanentVerb), Prop(Buff)];

    /// <summary>Absent for a buff that carries its own verb: "can attack as though it didn't have defender".</summary>
    [Optional]
    public PermanentVerb? PermanentVerb { get; set; }

    public Buff Buff { get; set; }
}
