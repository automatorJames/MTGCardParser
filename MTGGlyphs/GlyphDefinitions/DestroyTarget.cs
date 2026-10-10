namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Destroying or exiling one permanent: "destroy target land", "exile target creature", "destroy that creature at end of combat", and as a cost "exile {this}". The verb is a <see cref="RemovalVerb"/>.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Destroyed), Prop(AtEndOfCombat)];

    public RemovalVerb Verb { get; set; }
    public PermanentPhrase Destroyed { get; set; }
    [RegexPattern("at end of combat")]
    public bool AtEndOfCombat { get; set; }
}
