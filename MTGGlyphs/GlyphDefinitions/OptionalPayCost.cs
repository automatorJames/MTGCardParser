namespace MTGGlyphs.GlyphDefinitions;

public class OptionalPayCost : Glyph
{
    public override Nib[] Nibs => [Prop(PayOptionType), "pay", Prop(Cost)];

    public PayOptionType PayOptionType { get; set; }
    public Cost Cost { get; set; }
}

public enum PayOptionType
{
    UnlessYou,
    YouMay
}