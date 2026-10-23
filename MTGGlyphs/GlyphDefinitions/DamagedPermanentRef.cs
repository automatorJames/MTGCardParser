namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to that creature", "to it": the permanent damage was dealt to, named again. Only back-references, since anything wider here is expanded wherever an amount can go.</summary>
/// <exampledoc>Torch the Witness</exampledoc>
/// <examplecapture>to that creature</examplecapture>
[Dependent]
public class DamagedPermanentRef : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Permanent)];

    public OneOf<ThatCard, It> Permanent { get; set; }
}
