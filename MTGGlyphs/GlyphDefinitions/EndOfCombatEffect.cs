namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An action put off to the end of the current combat: "sacrifice it at end of combat", "return it to its owner's hand at end of combat", "remove a +1/+1 counter from it at end of combat", or with who does it, "this creature deals 1 damage to that creature at end of combat", "that creature's controller sacrifices it at end of combat". The action is a predicate, so a conditional sentence keeps its condition outermost.</summary>
/// <exampledoc>Crumbling Colossus</exampledoc>
/// <examplecapture>sacrifice it at end of combat</examplecapture>
public class EndOfCombatEffect : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Prop(Effect), Prop(When)];

    [Optional]
    public Subject Subject { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Effect { get; set; }
    public AtEndOfCombat When { get; set; }
}
