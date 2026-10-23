namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The leading items of a serial list of qualifiers, with the serial comma before its conjunction: "artifact, creature," in "artifact, creature, and/or enchantment". A separate glyph because the comma can't be an optional nib after a comma-joined list.</summary>
/// <exampledoc>Brightglass Gearhulk</exampledoc>
/// <examplecapture>artifact, creature,</examplecapture>
[Dependent]
public class SerialQualifiers : Glyph
{
    public override Nib[] Nibs => [Prop(Items), ","];

    public CompoundOf<TargetQualifier> Items { get; set; }
}
