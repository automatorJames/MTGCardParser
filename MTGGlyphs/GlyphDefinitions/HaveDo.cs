namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Making another object or player do something: "have target creature get -2/-2 until end of turn", "have each opponent lose 1 life", "have it deal that much damage to target creature" (usually after "you may").</summary>
/// <exampledoc>Blightcaster</exampledoc>
/// <examplecapture>have target creature get -2/-2 until end of turn</examplecapture>
[Dependent]
public class HaveDo : Glyph, IPredicate
{
    public override Nib[] Nibs => ["have", Prop(Doer), Prop(Action)];

    public Subject Doer { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Action { get; set; }
}
