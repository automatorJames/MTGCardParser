namespace MTGGlyphs.GlyphDefinitions;

/// <summary>`in addition to its other types and gains "…"`: an animated permanent keeping its types and granted a quoted ability, as in `becomes a bear creature in addition to its other types and gains "…"`. The closing quote falls after the period.</summary>
/// <exampledoc>Beorn's Hospitality</exampledoc>
/// <examplecapture>in addition to its other types and gains "this creature's power and toughness are each equal to the number of lands you control</examplecapture>
[Dependent]
public class InAdditionAndGainsQuoted : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["in addition to ", Alt("its", "their"), " other types and gains \"", Prop(Ability)];

    public DynamicGlyph Ability { get; set; }
}
