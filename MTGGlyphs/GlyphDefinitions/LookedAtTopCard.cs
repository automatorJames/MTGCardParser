namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top card of target player's library".</summary>
/// <exampledoc>Dewdrop Spy</exampledoc>
/// <examplecapture>the top card of target player's library</examplecapture>
[Dependent]
public class LookedAtTopCard : Glyph
{
    public override Nib[] Nibs => ["the top card of", Prop(Owner), "library"];

    public Owner Owner { get; set; }
}
