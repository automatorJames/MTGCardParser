namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature counting as more powerful for crewing or saddling: "crews vehicles as though its power were 2 greater", "saddles mounts and crews vehicles as though its power were 2 greater" (pilot tokens).</summary>
/// <exampledoc>Hotshot Mechanic</exampledoc>
/// <examplecapture>crews vehicles as though its power were 2 greater</examplecapture>
[Dependent]
public class CrewsAsThoughPowerGreater : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), "as though its power were", Prop(Amount), "greater"];

    public CrewAction Action { get; set; }
    public Quantity Amount { get; set; }
}
