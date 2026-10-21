namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A planeswalker's emblem: "you get an emblem with "…"", the quoted ability being any glyph that resolves. The closing quote falls after the sentence's period.</summary>
/// <exampledoc>Elspeth, Sun's Champion</exampledoc>
/// <examplecapture>you get an emblem with "creatures you control get +2/+2 and have flying</examplecapture>
public class GetEmblem : Glyph, IPredicate
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["get an emblem with \"", Prop(Ability)];

    public DynamicGlyph Ability { get; set; }
}
