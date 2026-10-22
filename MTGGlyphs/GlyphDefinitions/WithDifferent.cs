namespace MTGGlyphs.GlyphDefinitions;

/// <summary>No two of a group sharing a characteristic: "with different powers", "with different names", "with different mana values".</summary>
/// <exampledoc>Contortionist Troupe</exampledoc>
/// <examplecapture>with different powers</examplecapture>
[Dependent]
public class WithDifferent : Glyph
{
    public override Nib[] Nibs => ["with different", Plural(Prop(Characteristic))];

    public Characteristic Characteristic { get; set; }
}
