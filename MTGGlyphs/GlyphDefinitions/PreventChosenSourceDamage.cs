namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A shield against one chosen source: "the next time a red source of your choice would deal damage to you this turn, prevent that damage" - the Circle of Protection template.</summary>
/// <exampledoc>Circle of Protection: Red</exampledoc>
/// <examplecapture>the next time a red source of your choice would deal damage to you this turn, prevent that damage</examplecapture>
public class PreventChosenSourceDamage : Glyph
{
    public override Nib[] Nibs => ["the next time", Prop(Source), "would deal damage to", Prop(Protected), "this turn, prevent that damage"];

    public ChosenSource Source { get; set; }
    public Recipient Protected { get; set; }
}
