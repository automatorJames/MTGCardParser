namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A standing permission to look at hidden cards whenever you like: "look at the top card of your library any time", "look at face-down creatures you don't control any time" (in "you may …").</summary>
/// <exampledoc>Sphinx of Jwar Isle</exampledoc>
/// <examplecapture>look at the top card of your library any time</examplecapture>
[Dependent]
public class LookAtAnyTime : Glyph, IPredicate
{
    public override Nib[] Nibs => ["look at", Prop(What), "any time"];

    public OneOf<LookedAtThing, Permanents> What { get; set; }
}
