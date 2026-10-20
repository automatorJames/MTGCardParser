namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Toxic's reminder text, with the number of poison counters as a slot.</summary>
/// <exampledoc>Nimraiser Paladin</exampledoc>
/// <examplecapture>players dealt combat damage by this creature also get two poison counters</examplecapture>
public class ToxicRule : Glyph
{
    public override Nib[] Nibs => ["players dealt combat damage by this creature also get", Prop(Counters), Plural("poison counter")];

    public Quantity Counters { get; set; }
}
