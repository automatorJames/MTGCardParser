namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"attach {this} to it", "attach it to target creature you control", "attach this equipment to that creature": an aura or equipment being attached to a permanent. Also a predicate, so "you may attach it to this creature" resolves.</summary>
/// <exampledoc>Piston Sledge</exampledoc>
/// <examplecapture>attach it to target creature you control</examplecapture>
public class Attach : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["attach", Prop(Attachment), "to", Prop(AttachedTo)];

    public OneOf<This, ThisCreature, It> Attachment { get; set; }
    public PermanentPhrase AttachedTo { get; set; }
}
