namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Exerting a creature as it attacks, an optional choice: "exert this creature as it attacks" (in "you may …"). An exerted creature won't untap during its controller's next untap step.</summary>
/// <exampledoc>Battlefield Scavenger</exampledoc>
/// <examplecapture>exert this creature as it attacks</examplecapture>
[Dependent]
public class ExertAsItAttacks : Glyph, IPredicate
{
    public override Nib[] Nibs => ["exert", Prop(Creature), "as it attacks"];

    public PermanentPhrase Creature { get; set; }
}
