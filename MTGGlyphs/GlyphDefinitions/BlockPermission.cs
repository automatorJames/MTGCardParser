namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a creature may block, beyond or instead of the usual one attacker: "can block only creatures with flying", "can block an additional creature each combat", "can block any number of creatures this turn".</summary>
/// <exampledoc>Foriysian Brigade</exampledoc>
/// <examplecapture>can block an additional creature each combat</examplecapture>
[Dependent]
public class BlockPermission : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["can block", Prop(Scope), Prop(Period)];

    public OneOf<OnlyCreaturesWith, BlockCount?> Scope { get; set; }
    [Optional]
    public BlockPeriod? Period { get; set; }
}
