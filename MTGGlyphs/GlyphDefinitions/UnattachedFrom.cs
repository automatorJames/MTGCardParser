namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"from target creature", "from them", "from a creature you control": the permanent an aura or equipment is unattached from.</summary>
/// <exampledoc>Disarm</exampledoc>
/// <examplecapture>from target creature</examplecapture>
[Dependent]
public class UnattachedFrom : Glyph
{
    public override Nib[] Nibs => ["from", Prop(Holder)];

    public OneOf<PermanentPhrase, IndefinitePermanent> Holder { get; set; }
}
