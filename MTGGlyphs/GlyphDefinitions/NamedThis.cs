namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A name qualifier on objects: "named {this}", "not named {this}" - other objects sharing (or not) this card's name.</summary>
/// <exampledoc>Relentless Rats</exampledoc>
/// <examplecapture>named {this}</examplecapture>
[Dependent]
public class NamedThis : Glyph
{
    public override Nib[] Nibs => [Prop(Not), "named", Nib.This];

    [Optional]
    [RegexPattern("not")]
    public bool Not { get; set; }
}
