namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent entering or attacking, as the moment a replacement applies: "{this} enters", "this creature attacks".</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>{this} enters</examplecapture>
[Dependent]
public class PermanentAsEvent : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), Prop(Event)];

    public PermanentPhrase Permanent { get; set; }
    public AsPermanentEvent Event { get; set; }
}
