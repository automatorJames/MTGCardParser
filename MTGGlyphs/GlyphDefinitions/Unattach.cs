namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"unattach it", "unattach enchanted equipment", "unattach elbrus" (a legend's short name), "unattach all equipment from target creature", "unattach an equipment from a creature you control": an aura or equipment coming off what it's attached to, staying on the battlefield. Also a predicate, so "you may unattach …" resolves.</summary>
/// <exampledoc>Disarm</exampledoc>
/// <examplecapture>unattach all equipment from target creature</examplecapture>
public class Unattach : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["unattach", Prop(Attachment), Prop(From)];

    public OneOf<PermanentPhrase, ShortName, OneOf<IndefinitePermanent, CountedPermanents>> Attachment { get; set; }
    [Optional]
    public UnattachedFrom From { get; set; }
}
