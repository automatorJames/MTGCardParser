namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What a redirected spell or ability now targets: "to this creature", "to enchanted creature".</summary>
/// <exampledoc>Spellskite</exampledoc>
/// <examplecapture>to this creature</examplecapture>
[Dependent]
public class NewTargetIs : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Target)];

    public OneOf<ThisCreature, EnchantedPermanent> Target { get; set; }
}
