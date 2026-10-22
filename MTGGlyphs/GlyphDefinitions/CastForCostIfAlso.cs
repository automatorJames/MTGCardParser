namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost that adds an action to its payment: "you may cast this spell for {w} if you also return a tapped creature you control to its owner's hand" (web-slinging).</summary>
/// <exampledoc>Spider-UK</exampledoc>
/// <examplecapture>you may cast this spell for {2}{w} if you also return a tapped creature you control to its owner's hand</examplecapture>
public class CastForCostIfAlso : Glyph
{
    public override Nib[] Nibs => ["you may cast this spell for", Prop(Cost), "if you also", Prop(Action)];

    public ManaCost Cost { get; set; }
    [TypeFilter(typeof(IPredicate))]
    public DynamicGlyph Action { get; set; }
}
