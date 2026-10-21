namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount scaled by a factor, without rounding: "twice the number of cards in your hand".</summary>
/// <exampledoc>Dwarven Shrine</exampledoc>
/// <examplecapture>twice the number of cards in all graveyards with the same name as that spell</examplecapture>
[Dependent]
public class ScaledAmountExact : Glyph
{
    public override Nib[] Nibs => [Prop(Factor), Prop(Base)];

    public ScaleFactor Factor { get; set; }
    public Amount Base { get; set; }
}
