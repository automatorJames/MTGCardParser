namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Plot's reminder text, first sentence, with the plot cost repeated as a slot.</summary>
/// <exampledoc>Outcaster Trailblazer</exampledoc>
/// <examplecapture>you may pay {2}{g} and exile this card from your hand</examplecapture>
public class PlotRule : Glyph
{
    public override Nib[] Nibs => ["you may pay", Prop(Cost), "and exile this card from your hand"];

    public ManaCost Cost { get; set; }
}
