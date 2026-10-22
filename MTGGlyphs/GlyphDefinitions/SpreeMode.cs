namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One mode of a spree spell, with the additional cost of choosing it: "+ {3} — target player draws two cards".</summary>
/// <exampledoc>Metamorphic Blast</exampledoc>
/// <examplecapture>+ {3} — target player draws two cards</examplecapture>
public class SpreeMode : Glyph
{
    public override Nib[] Nibs => ["+", Prop(Cost), "—", Prop(Effect)];

    public ManaCost Cost { get; set; }
    public DynamicGlyph Effect { get; set; }
}
