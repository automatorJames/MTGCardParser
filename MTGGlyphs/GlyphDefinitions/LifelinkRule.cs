namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Lifelink's reminder text: "damage dealt by this creature also causes you to gain that much life", and its granted forms: "damage dealt by a creature with lifelink also causes its controller to gain that much life", "damage dealt by the creature also causes its controller to gain that much life", "damage dealt by those creatures also causes you to gain that much life". What deals the damage, and who gains the life.</summary>
/// <exampledoc>Abzan Guide</exampledoc>
/// <examplecapture>damage dealt by this creature also causes you to gain that much life</examplecapture>
public class LifelinkRule : Glyph
{
    public override Nib[] Nibs => ["damage dealt by", Prop(Source), "also causes", Prop(Gainer), "to gain that much life"];

    public Subject Source { get; set; }
    public OneOf<WhichPlayer?, PlayerReference> Gainer { get; set; }
}
