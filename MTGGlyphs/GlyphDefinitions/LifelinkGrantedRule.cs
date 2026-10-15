namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Lifelink's reminder text, for a granted ability.</summary>
/// <exampledoc>Abzan Guide</exampledoc>
/// <examplecapture>damage dealt by the creature also causes its controller to gain that much life</examplecapture>
public class LifelinkGrantedRule : Glyph
{
    public override Nib[] Nibs => ["damage dealt by the creature also causes its controller to gain that much life"];
}
