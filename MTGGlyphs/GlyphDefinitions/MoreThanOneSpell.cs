namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"more than one spell each turn": a limit of one spell per turn.</summary>
/// <exampledoc>Colfenor's Plans</exampledoc>
/// <examplecapture>more than one spell each turn</examplecapture>
[Dependent]
public class MoreThanOneSpell : Glyph
{
    public override Nib[] Nibs => ["more than one spell each turn"];
}
