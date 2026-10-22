namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with that name", "with the chosen name": cards picked out by a card name chosen or named earlier.</summary>
/// <exampledoc>Memoricide</exampledoc>
/// <examplecapture>with that name</examplecapture>
[Dependent]
public class WithName : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Name)];

    public NamedReference Name { get; set; }
}
