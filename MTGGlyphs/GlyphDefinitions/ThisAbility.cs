namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this ability": the ability being read, referred to by itself.</summary>
/// <exampledoc>Cryptoplasm</exampledoc>
/// <examplecapture>this ability</examplecapture>
[Dependent]
public class ThisAbility : Glyph
{
    public override Nib[] Nibs => ["this ability"];
}
