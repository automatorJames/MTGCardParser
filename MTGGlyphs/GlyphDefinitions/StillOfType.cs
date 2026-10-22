namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that's still a land", "that's still a planeswalker": an animated permanent keeping its own card type.</summary>
/// <exampledoc>Gideon Jura</exampledoc>
/// <examplecapture>that's still a planeswalker</examplecapture>
[Dependent]
public class StillOfType : Glyph
{
    public override Nib[] Nibs => ["that's still", Pattern("an?"), Prop(Kept)];

    public CardType Kept { get; set; }
}
