namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Hexproof's and shroud's reminder predicate: "can't be the target of spells or abilities", optionally "your opponents control".</summary>
/// <exampledoc>Ascended Lawmage</exampledoc>
/// <examplecapture>can't be the target of spells or abilities your opponents control</examplecapture>
[Dependent]
public class CantBeTargeted : Glyph, IPredicate
{
    public override Nib[] Nibs => ["can't be the target of spells or abilities", Prop(Whose)];

    [Optional]
    public ControlledByWho Whose { get; set; }
}
