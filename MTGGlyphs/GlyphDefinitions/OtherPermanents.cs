namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"other creatures you control", "other white creatures", "other goblins you control": every permanent of a kind except the one with the ability, as a lord's subject.</summary>
/// <exampledoc>Angel of Invention</exampledoc>
/// <examplecapture>other creatures you control</examplecapture>
[Dependent]
public class OtherPermanents : Glyph
{
    public override Nib[] Nibs => ["other", Prop(Permanents)];

    public Permanents Permanents { get; set; }
}
