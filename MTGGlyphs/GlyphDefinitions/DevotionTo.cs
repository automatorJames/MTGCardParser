namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your devotion to black", "your devotion to green and white": the mana symbols of a color among the mana costs of permanents you control.</summary>
/// <exampledoc>Marshmist Titan</exampledoc>
/// <examplecapture>your devotion to black</examplecapture>
[Dependent]
public class DevotionTo : Glyph
{
    public override Nib[] Nibs => ["your devotion to", Prop(Colors)];

    public OneOf<ManyOf<ManaColor>, ManaColor?> Colors { get; set; }
}
