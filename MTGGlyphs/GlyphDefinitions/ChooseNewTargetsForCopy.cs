namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"you may choose new targets for the copy" / "you may choose a new target for the copy": a copy of a spell.</summary>
/// <exampledoc>Twincast</exampledoc>
/// <examplecapture>you may choose new targets for the copy</examplecapture>
public class ChooseNewTargetsForCopy : Glyph
{
    public override Nib[] Nibs => ["you may choose", Alt("new targets", "a new target"), "for the copy"];
}
