namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A restriction on the mana spent on a spell's or ability's {x}: "spend only black mana on x", "spend only colored mana on x".</summary>
/// <exampledoc>Consume Spirit</exampledoc>
/// <examplecapture>spend only black mana on x</examplecapture>
public class SpendOnlyManaOnX : Glyph
{
    public override Nib[] Nibs => ["spend only", Prop(Color), "mana on x"];

    public OneOf<ManaColor?, Colorfulness?> Color { get; set; }
}
