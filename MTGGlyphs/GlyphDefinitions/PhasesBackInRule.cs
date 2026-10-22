namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Phasing's reminder of when a phased-out permanent returns: "it phases in before you untap during your next untap step", "it phases in before its controller untaps during their next untap step", "they phase in before that player untaps during their next untap step", "each one phases in before its controller untaps …".</summary>
/// <exampledoc>Mist Dragon</exampledoc>
/// <examplecapture>it phases in before you untap during your next untap step</examplecapture>
public class PhasesBackInRule : Glyph
{
    public override Nib[] Nibs => [Prop(Phased), Pattern("phases?"), "in before", Prop(Player), Pattern("untaps?"), "during", Pattern("(your|their)"), "next untap step"];

    public OneOf<Subject, EachOne> Phased { get; set; }
    public OneOf<WhichPlayer?, PlayerReference> Player { get; set; }
}
