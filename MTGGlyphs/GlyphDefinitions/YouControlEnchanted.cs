namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An aura that gives its controller control of the enchanted permanent: "you control enchanted creature".</summary>
/// <exampledoc>Abduction</exampledoc>
/// <examplecapture>you control enchanted creature</examplecapture>
public class YouControlEnchanted : Glyph
{
    public override Nib[] Nibs => ["you control", Prop(Enchanted)];

    public EnchantedPermanent Enchanted { get; set; }
}
