namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect or rule followed by "and the damage can't be prevented": "this spell can't be countered and the damage can't be prevented", "{this} deals 5 damage to that permanent or player and the damage can't be prevented".</summary>
/// <exampledoc>Banefire</exampledoc>
/// <examplecapture>this spell can't be countered and the damage can't be prevented</examplecapture>
[Dependent]
public class AndDamageCantBePrevented : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), "and", Prop(Rule)];

    public DynamicGlyph Effect { get; set; }
    public DamageCantBePrevented Rule { get; set; }
}
