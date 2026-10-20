namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder that mana abilities can't be countered by "counter target activated ability": "mana abilities can't be targeted".</summary>
/// <exampledoc>Stifle</exampledoc>
/// <examplecapture>mana abilities can't be targeted</examplecapture>
public class ManaAbilitiesCantBeTargeted : Glyph
{
    public override Nib[] Nibs => ["mana abilities can't be targeted"];
}
