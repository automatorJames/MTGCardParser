namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Haunt's trigger: "when this creature enters or the creature it haunts dies, …" - fires once on entering and again when the haunted creature dies - or, for a haunting spell, "when the creature this card haunts dies, …".</summary>
/// <exampledoc>Absolver Thrull</exampledoc>
/// <examplecapture>when this creature enters or the creature it haunts dies, destroy target enchantment</examplecapture>
public class HauntTrigger : Glyph
{
    public override Nib[] Nibs => ["when", Prop(Enters), "the creature", Alt("it haunts", "this card haunts"), "dies,", Prop(Effect)];

    [Optional]
    public HaunterEnters Enters { get; set; }
    public DynamicGlyph Effect { get; set; }
}
