namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"during your turn", "during turns other than yours": a static effect limited to some players' turns.</summary>
/// <exampledoc>Sunlit Hoplite</exampledoc>
/// <examplecapture>during your turn</examplecapture>
[Dependent]
public class DuringTurn : Glyph
{
    public override Nib[] Nibs => ["during", Prop(Turn)];

    public TurnOwner Turn { get; set; }
}
