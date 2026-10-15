namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"until {this} leaves the battlefield": an exile that lasts only as long as this permanent stays.</summary>
/// <exampledoc>Banisher Priest</exampledoc>
/// <examplecapture>until {this} leaves the battlefield</examplecapture>
[Dependent]
public class UntilThisLeaves : Glyph
{
    public override Nib[] Nibs => ["until", Nib.This, "leaves the battlefield"];
}
