namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object named again by what happened to it, in a relative clause: "the creature that died", "the card you exiled", "the creature you chose", "the card you revealed" - for amounts taken from it ("where x is the power of the creature that died").</summary>
/// <exampledoc>Death's Presence</exampledoc>
/// <examplecapture>the creature that died</examplecapture>
[Dependent]
public class ObjectNamedByEvent : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Kind), Prop(Event)];

    public OneOf<CardType?, CardNoun?> Kind { get; set; }
    public ObjectEventClause Event { get; set; }
}
