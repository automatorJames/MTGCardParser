namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A punisher choice: the effect happens unless a player has a source deal damage to them (or do something else) instead: "unless its controller has {this} deal 6 damage to them", "unless a player has this creature deal 4 damage to them".</summary>
/// <exampledoc>Lava Blister</exampledoc>
/// <examplecapture>unless its controller has {this} deal 6 damage to them</examplecapture>
[Dependent]
public class UnlessHasDo : Glyph
{
    public override Nib[] Nibs => [Prop(Player), "has", Prop(Doer), Prop(Action)];

    public UnlessPlayer Player { get; set; }
    public Subject Doer { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Action { get; set; }
}
