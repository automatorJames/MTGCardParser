namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Explore's rule for a nonland card: "otherwise, put a +1/+1 counter on this creature, then put the card back or put it into your graveyard".</summary>
/// <exampledoc>Dire Fleet Interloper</exampledoc>
/// <examplecapture>otherwise, put a +1/+1 counter on this creature, then put the card back or put it into your graveyard</examplecapture>
public class ExploreNonlandRule : Glyph
{
    public override Nib[] Nibs => ["otherwise,", Prop(Counter), ", then put the card back or put it into your graveyard"];

    public PlaceCounters Counter { get; set; }
}
