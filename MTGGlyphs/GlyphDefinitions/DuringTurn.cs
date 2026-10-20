namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"during your turn", "during an opponent's turn": whose turn something happens in.</summary>
/// <exampledoc>Dreamspoiler Witches</exampledoc>
/// <examplecapture>during an opponent's turn</examplecapture>
[Dependent]
public class DuringTurn : Glyph
{
    public override Nib[] Nibs => ["during", Prop(Whose), "turn"];

    public Whose Whose { get; set; }
}
