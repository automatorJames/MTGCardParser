namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"reduced by {2}": a discount on a mana cost being paid.</summary>
/// <exampledoc>Flash</exampledoc>
/// <examplecapture>reduced by {2}</examplecapture>
[Dependent]
public class ManaCostReduction : Glyph
{
    public override Nib[] Nibs => ["reduced by", Prop(By)];

    public ManaCost By { get; set; }
}
