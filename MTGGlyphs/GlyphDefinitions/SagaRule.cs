namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Saga reminder text: "as this saga enters and after your draw step, add a lore counter" - how a Saga advances through its chapters.</summary>
/// <exampledoc>The Binding of the Titans</exampledoc>
/// <examplecapture>as this saga enters and after your draw step, add a lore counter</examplecapture>
public class SagaRule : Glyph
{
    public override Nib[] Nibs => ["as this saga enters and after your draw step, add a lore counter"];
}
