namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"and/or {1}{b}": a second optional cost that may be paid along with the first or alone (the volvers' kicker).</summary>
/// <exampledoc>Anavolver</exampledoc>
/// <examplecapture>and/or {b}</examplecapture>
[Dependent]
public class AndOrCost : Glyph
{
    public override Nib[] Nibs => ["and/or", Prop(Cost)];

    public ManaCost Cost { get; set; }
}
