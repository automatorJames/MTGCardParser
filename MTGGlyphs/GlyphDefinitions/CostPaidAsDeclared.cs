namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The reminder on attack/block costs: "this cost is paid as attackers are declared", "this cost is paid as attackers or blockers are declared".</summary>
/// <exampledoc>Leviathan</exampledoc>
/// <examplecapture>this cost is paid as attackers are declared</examplecapture>
public class CostPaidAsDeclared : Glyph
{
    public override Nib[] Nibs => ["this cost is paid as", Prop(Declared), "are declared"];

    public OneOf<ManyOf<CombatRole>, CombatRole?> Declared { get; set; }
}
