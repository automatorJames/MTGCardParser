namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attach {this} to it", "attach it to target creature you control", "attach target equipment you control to target creature you control", "attach this aura to a creature you control" (an indefinite permanent, chosen on resolution), "attach nahiri's armor to …" (a legend's short name), "attach an equipment you control to it", "attach any number of equipment you control to it", "attach all equipment on the battlefield to it": an aura or equipment being attached to a permanent. Also a predicate, so "you may attach it to this creature" resolves.</summary>
/// <exampledoc>Piston Sledge</exampledoc>
/// <examplecapture>attach it to target creature you control</examplecapture>
public class Attach : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["attach", Prop(Attachment), "to", Prop(AttachedTo)];

    public OneOf<PermanentPhrase, ShortName, IndefinitePermanent, CountedPermanents> Attachment { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent> AttachedTo { get; set; }
}
