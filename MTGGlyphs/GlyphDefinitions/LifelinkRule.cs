namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Lifelink's reminder text: "damage dealt by this creature also causes you to gain that much life".</summary>
/// <exampledoc>Abzan Guide</exampledoc>
/// <examplecapture>damage dealt by this creature also causes you to gain that much life</examplecapture>
public class LifelinkRule : Glyph
{
    public override Nib[] Nibs => ["damage dealt by", Prop(Source), "also causes you to gain that much life"];

    public ThisCreature Source { get; set; }
}
