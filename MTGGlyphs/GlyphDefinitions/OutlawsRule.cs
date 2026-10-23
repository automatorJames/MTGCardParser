namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The definition of outlaws: "assassins, mercenaries, pirates, rogues, and warlocks are outlaws". Tried first, so the general "X are Y" reading doesn't take the rule.</summary>
/// <exampledoc>Vadmir, New Blood</exampledoc>
/// <examplecapture>assassins, mercenaries, pirates, rogues, and warlocks are outlaws</examplecapture>
[TokenizationOrder(0)]
public class OutlawsRule : Glyph
{
    public override Nib[] Nibs => ["assassins, mercenaries, pirates, rogues, and warlocks are outlaws"];
}
