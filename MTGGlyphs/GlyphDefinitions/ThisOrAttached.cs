namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger subject naming the card itself or the permanent it's attached to: "this creature or equipped creature", "this creature or enchanted creature" - a creature equipment or aura that triggers whichever of the two does it.</summary>
/// <exampledoc>Acquisition Octopus</exampledoc>
/// <examplecapture>this creature or equipped creature</examplecapture>
[Dependent]
public class ThisOrAttached : Glyph
{
    public override Nib[] Nibs => [Prop(Self), "or", Prop(Attached)];

    public OneOf<ThisCreature, This, ShortName> Self { get; set; }
    public EnchantedPermanent Attached { get; set; }
}
