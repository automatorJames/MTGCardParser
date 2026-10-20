namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"if you pay {2} more to cast it": an extra cost for casting a spell a special way.</summary>
/// <exampledoc>Breaking Wave</exampledoc>
/// <examplecapture>if you pay {2} more to cast it</examplecapture>
[Dependent]
public class PayMoreToCast : Glyph
{
    public override Nib[] Nibs => ["if you pay", Prop(Cost), "more to cast it"];

    public ManaCost Cost { get; set; }
}
