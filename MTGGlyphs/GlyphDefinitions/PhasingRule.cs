namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The phasing keyword's reminder: "this phases in or out before you untap during each of your untap steps", or for a granted ability, "it phases in or out before its controller untaps during each of their untap steps".</summary>
/// <exampledoc>Mist Dragon</exampledoc>
/// <examplecapture>this phases in or out before you untap during each of your untap steps</examplecapture>
public class PhasingRule : Glyph
{
    public override Nib[] Nibs => [Prop(Phased), "phases in or out before", Prop(Player), Pattern("untaps?"), "during each of", Pattern("(your|their)"), "untap steps"];

    public Subject Phased { get; set; }
    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
}
