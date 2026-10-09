namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice made as part of an effect: "choose an opponent", "choose a color and an opponent", "choose a basic land type".</summary>
/// <exampledoc>Jihad</exampledoc>
/// <examplecapture>choose a color and an opponent</examplecapture>
public class Choose : Glyph
{
    public override Nib[] Nibs => ["choose", Prop(Choices)];

    public OneOf<ManyOf<Choice>, Choice?> Choices { get; set; }
}
