namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"red mana", "colorless mana": mana of one named color.</summary>
/// <exampledoc>Sunglasses of Urza</exampledoc>
/// <examplecapture>red mana</examplecapture>
[Dependent]
public class ManaOfColor : Glyph
{
    public override Nib[] Nibs => [Prop(Color), "mana"];

    public ManaColor Color { get; set; }
}
