namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"named {this}": a card with the same name as this one.</summary>
/// <exampledoc>Borderland Ranger</exampledoc>
/// <examplecapture>named {this}</examplecapture>
[Dependent]
public class NamedThis : Glyph
{
    public override Nib[] Nibs => ["named", Nib.This];
}
