namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Toxic's reminder text, with the damage source and the number of poison counters as slots: "players dealt combat damage by this creature/it/them/equipped creature also get a poison counter".</summary>
/// <exampledoc>Nimraiser Paladin</exampledoc>
/// <examplecapture>players dealt combat damage by this creature also get two poison counters</examplecapture>
public class ToxicRule : Glyph
{
    public override Nib[] Nibs => ["players dealt combat damage by", Prop(Source), "also get", Prop(Counters), Plural("poison counter")];

    public OneOf<PermanentPhrase, Them> Source { get; set; }
    public Quantity Counters { get; set; }
}
