namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Destroying or exiling one permanent: "destroy target land", "exile target creature", "destroy that creature", and as a cost "exile {this}". The verb is a <see cref="RemovalVerb"/>.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Destroyed)];

    public RemovalVerb Verb { get; set; }
    public PermanentPhrase Destroyed { get; set; }
}
