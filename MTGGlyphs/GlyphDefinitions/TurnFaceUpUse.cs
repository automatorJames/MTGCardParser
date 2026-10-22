namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to turn face-down permanents face up: "turn permanents face up", "to turn creatures face up", "turn a permanent face up".</summary>
/// <exampledoc>Overgrown Zealot</exampledoc>
/// <examplecapture>turn permanents face up</examplecapture>
[Dependent]
public class TurnFaceUpUse : Glyph
{
    public override Nib[] Nibs => [Opt("to"), "turn", Prop(What), "face up"];

    public OneOf<IndefinitePermanent, Permanents> What { get; set; }
}
