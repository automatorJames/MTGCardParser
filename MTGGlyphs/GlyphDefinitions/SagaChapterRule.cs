namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Saga reminder sentences after read ahead's: "add one after your draw step", "skipped chapters don't trigger".</summary>
/// <exampledoc>The World Spell</exampledoc>
/// <examplecapture>add one after your draw step</examplecapture>
public class SagaChapterRule : Glyph
{
    public override Nib[] Nibs => [Alt("add one after your draw step", "skipped chapters don't trigger")];
}
