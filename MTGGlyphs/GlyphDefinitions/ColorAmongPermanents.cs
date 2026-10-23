namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"any color among legendary creatures and planeswalkers you control": a color shared with permanents of a kind.</summary>
/// <exampledoc>Mox Amber</exampledoc>
/// <examplecapture>any color among legendary creatures and planeswalkers you control</examplecapture>
[Dependent]
public class ColorAmongPermanents : Glyph
{
    public override Nib[] Nibs => ["any color among", Prop(Permanents)];

    public Permanents Permanents { get; set; }
}
