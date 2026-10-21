namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The creatures in a player's party (one each of cleric, rogue, warrior and wizard), as something counted: "for each creature in your party", "the number of creatures in your party".</summary>
/// <exampledoc>Tazri, Beacon of Unity</exampledoc>
/// <examplecapture>creature in your party</examplecapture>
[Dependent]
public class PartyCreatures : Glyph
{
    public override Nib[] Nibs => [Plural("creature"), "in", Prop(Whose), "party"];

    public Whose Whose { get; set; }
}
