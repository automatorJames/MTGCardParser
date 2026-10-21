namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which of several kickers was paid: "with its {2}{u} kicker".</summary>
/// <exampledoc>Ana Battlemage</exampledoc>
/// <examplecapture>with its {2}{u} kicker</examplecapture>
[Dependent]
public class WithItsKicker : Glyph
{
    public override Nib[] Nibs => ["with its", Prop(Cost), "kicker"];

    public ManaCost Cost { get; set; }
}
