namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A limit on untapping: "players can't untap more than one artifact during their untap steps".</summary>
/// <exampledoc>Damping Field</exampledoc>
/// <examplecapture>players can't untap more than one artifact during their untap steps</examplecapture>
public class UntapLimit : Glyph
{
    public override Nib[] Nibs => ["players can't untap more than one", Prop(Kind), "during their untap steps"];

    public CardType Kind { get; set; }
}
