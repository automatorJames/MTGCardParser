namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Vigilance's reminder text, for whatever has vigilance: "attacking doesn't cause this creature to tap", "attacking doesn't cause it to tap", "attacking doesn't cause them to tap".</summary>
/// <exampledoc>Brushstrider</exampledoc>
/// <examplecapture>attacking doesn't cause this creature to tap</examplecapture>
public class VigilanceRule : Glyph
{
    public override Nib[] Nibs => ["attacking doesn't cause", Prop(Attacker), "to tap"];

    public OneOf<ThisCreature, It, Them> Attacker { get; set; }
}
