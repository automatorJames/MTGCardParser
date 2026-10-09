namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The optional-untap static: "you may choose not to untap {this} during your untap step".</summary>
/// <exampledoc>Ashnod's Battle Gear</exampledoc>
/// <examplecapture>you may choose not to untap {this} during your untap step</examplecapture>
public class MayChooseNotToUntap : Glyph
{
    public override Nib[] Nibs => ["you may choose not to untap", Prop(Permanent), "during", Prop(Step)];

    public PermanentPhrase Permanent { get; set; }
    public PlayersPhase Step { get; set; }
}
