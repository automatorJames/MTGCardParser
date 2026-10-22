namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Haunt's paired trigger: "when this creature enters or the creature it haunts dies, …" - fires once on entering and again when the haunted creature dies.</summary>
/// <exampledoc>Absolver Thrull</exampledoc>
/// <examplecapture>when this creature enters or the creature it haunts dies, destroy target enchantment</examplecapture>
public class HauntTrigger : Glyph
{
    public override Nib[] Nibs => ["when", Prop(Haunter), "enters or the creature it haunts dies,", Prop(Effect)];

    public ThisCreature Haunter { get; set; }
    public DynamicGlyph Effect { get; set; }
}
