namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature whose power and toughness are defined by a count: "{this}'s power and toughness are each equal to the number of swamps you control".</summary>
/// <exampledoc>Nightmare</exampledoc>
/// <examplecapture>{this}'s power and toughness are each equal to the number of swamps you control</examplecapture>
public class DefinedPowerToughness : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Nib.This, "'s power and toughness are each equal to ", Prop(Amount)];

    [AllowUnmatched]
    public DynamicGlyph Amount { get; set; }
}
