namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cap on how many times an ability may be activated each turn: "activate no more than twice each turn", "activate no more than three times each turn".</summary>
/// <exampledoc>Phyrexian Battleflies</exampledoc>
/// <examplecapture>activate no more than twice each turn</examplecapture>
public class ActivationLimit : Glyph
{
    public override Nib[] Nibs => ["activate no more than", Prop(Times), "each turn"];

    public CopyRepetition Times { get; set; }
}
