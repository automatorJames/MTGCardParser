namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attach {this} to it", "attach it to target creature you control", "attach target equipment you control to target creature you control", "attach this aura to a creature you control" (an indefinite permanent, chosen on resolution): an aura or equipment being attached to a permanent. Also a predicate, so "you may attach it to this creature" resolves.</summary>
/// <exampledoc>Piston Sledge</exampledoc>
/// <examplecapture>attach it to target creature you control</examplecapture>
public class Attach : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["attach", Prop(Attachment), "to", Prop(AttachedTo)];

    public PermanentPhrase Attachment { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent> AttachedTo { get; set; }
}
