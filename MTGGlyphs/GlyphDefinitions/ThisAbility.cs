namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this ability", as what a permanent loses: "loses this ability" (a Licid giving up the ability that turned it into an aura).</summary>
/// <exampledoc>Calming Licid</exampledoc>
/// <examplecapture>this ability</examplecapture>
[Dependent]
public class ThisAbility : Glyph
{
    public override Nib[] Nibs => ["this ability"];
}
