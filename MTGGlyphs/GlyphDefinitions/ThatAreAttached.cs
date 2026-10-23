namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that are enchanted", "that are equipped", "that's enchanted": permanents narrowed to those with an aura or equipment attached.</summary>
/// <exampledoc>Beastmaster's Magemark</exampledoc>
/// <examplecapture>that are enchanted</examplecapture>
[Dependent]
public class ThatAreAttached : Glyph
{
    public override Nib[] Nibs => [Alt("that are", "that's"), Prop(Attachment)];

    public AttachedBy Attachment { get; set; }
}
