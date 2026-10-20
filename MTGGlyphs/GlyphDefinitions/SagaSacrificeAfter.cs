namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Saga reminder text's second sentence: "sacrifice after iii" - the chapter after which the Saga is sacrificed.</summary>
/// <exampledoc>The Binding of the Titans</exampledoc>
/// <examplecapture>sacrifice after iii</examplecapture>
public class SagaSacrificeAfter : Glyph
{
    public override Nib[] Nibs => ["sacrifice after", Prop(Chapter)];

    public SagaChapter Chapter { get; set; }
}
