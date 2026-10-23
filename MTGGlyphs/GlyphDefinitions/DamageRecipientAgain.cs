namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to that creature", "to it": the recipient of replacement damage, named again by a back-reference to the original one.</summary>
/// <exampledoc>Cinder Strike</exampledoc>
/// <examplecapture>to that creature</examplecapture>
[Dependent]
public class DamageRecipientAgain : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Recipient)];

    public OneOf<ThatCard, It> Recipient { get; set; }
}
