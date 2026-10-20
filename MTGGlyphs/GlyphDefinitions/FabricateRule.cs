namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Fabricate's reminder text, with the fabricate amount as a slot (counters and servos alike).</summary>
/// <exampledoc>Angel of Invention</exampledoc>
/// <examplecapture>when this creature enters, put two +1/+1 counters on it or create two 1/1 colorless servo artifact creature tokens</examplecapture>
public class FabricateRule : Glyph
{
    public override Nib[] Nibs => ["when this creature", Alt("enters", "enters the battlefield"), ", put", Prop(Counters), Plural("+1/+1 counter"), "on it or create", Prop(Tokens), "1/1 colorless servo artifact creature", Plural("token")];

    public Quantity Counters { get; set; }
    public Quantity Tokens { get; set; }
}
