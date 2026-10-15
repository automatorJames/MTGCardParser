namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Destroying or exiling one permanent or several: "destroy target land", "exile two target artifacts", "exile all creatures", "exile target creature an opponent controls until {this} leaves the battlefield", "destroy that creature at end of combat", and as a cost "exile {this}". The verb is a <see cref="RemovalVerb"/>.</summary>
/// <exampledoc>Ice Storm</exampledoc>
/// <examplecapture>destroy target land</examplecapture>
public class DestroyTarget : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Destroyed), Prop(Until), Prop(AtEndOfCombat)];

    public RemovalVerb Verb { get; set; }
    public OneOf<PermanentPhrase, PermanentGroup> Destroyed { get; set; }
    [Optional]
    public UntilThisLeaves Until { get; set; }
    [RegexPattern("at end of combat")]
    public bool AtEndOfCombat { get; set; }
}
