namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"named {this}": cards or permanents sharing the card's name.</summary>
/// <exampledoc>Accumulated Knowledge</exampledoc>
/// <examplecapture>named {this}</examplecapture>
[Dependent]
public class NamedThis : Glyph
{
    public override Nib[] Nibs => ["named", Nib.This];
}
