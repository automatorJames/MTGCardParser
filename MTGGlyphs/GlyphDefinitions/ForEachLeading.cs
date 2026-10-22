namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect repeated once per counted thing, the count leading: "for each creature exiled this way, its controller draws a card", "for each land you control, create a treasure token", "for each opponent, gain control of …".</summary>
/// <exampledoc>Martyr's Cry</exampledoc>
/// <examplecapture>for each creature exiled this way, its controller draws a card</examplecapture>
public class ForEachLeading : Glyph
{
    public override Nib[] Nibs => [Prop(Multiplier), ",", Prop(Effect)];

    public ForEach Multiplier { get; set; }
    public DynamicGlyph Effect { get; set; }
}
