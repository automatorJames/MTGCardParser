namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top card of your library", "the top card of target player's library": the single top card of a library, as what's looked at, revealed, exiled or put somewhere.</summary>
/// <exampledoc>Sage Owl</exampledoc>
/// <examplecapture>the top card of your library</examplecapture>
[Dependent]
public class TopCardOfLibrary : Glyph
{
    public override Nib[] Nibs => ["the top card of", Prop(Owner), "library"];

    public Owner Owner { get; set; }
}
