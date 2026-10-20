namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Multikicker's reminder: "you may pay an additional {1}{w} any number of times as you cast this spell".</summary>
/// <exampledoc>Apex Hawks</exampledoc>
/// <examplecapture>you may pay an additional {1}{w} any number of times as you cast this spell</examplecapture>
public class MultikickerRule : Glyph
{
    public override Nib[] Nibs => ["you may pay an additional", Prop(Cost), "any number of times as you cast this spell"];

    public ManaCost Cost { get; set; }
}
