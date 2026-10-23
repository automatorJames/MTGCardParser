namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger-doubling static ability keyed to the ability's source rather than to an event: "if a triggered ability of a legendary creature you control triggers, that ability triggers an additional time", "if a triggered ability of another elemental you control triggers, it triggers an additional time", "if a triggered ability of equipped creature triggers, …".</summary>
/// <exampledoc>Annie Joins Up</exampledoc>
/// <examplecapture>if a triggered ability of a legendary creature you control triggers, that ability triggers an additional time</examplecapture>
public class SourceTriggersAgain : Glyph
{
    public override Nib[] Nibs => ["if a triggered ability of", Prop(Source), "triggers,", Alt("that ability", "it"), "triggers an additional time"];

    public OneOf<IndefinitePermanent, EnchantedPermanent> Source { get; set; }
}
