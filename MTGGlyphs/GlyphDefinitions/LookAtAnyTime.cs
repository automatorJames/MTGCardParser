namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A standing permission to look at hidden cards whenever you like: "look at the top card of your library any time" (in "you may …").</summary>
/// <exampledoc>Sphinx of Jwar Isle</exampledoc>
/// <examplecapture>look at the top card of your library any time</examplecapture>
[Dependent]
public class LookAtAnyTime : Glyph, IPredicate
{
    public override Nib[] Nibs => ["look at", Prop(What), "any time"];

    public LookedAtThing What { get; set; }
}
