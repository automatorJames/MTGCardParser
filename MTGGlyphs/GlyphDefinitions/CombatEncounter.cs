namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature meeting another in combat: "blocks or becomes blocked by a non-wall creature", "becomes blocked by a wall", "blocks a creature", "blocks or becomes blocked by one or more black creatures". The other creature is a referent, for a later "that creature".</summary>
/// <exampledoc>Cockatrice</exampledoc>
/// <examplecapture>blocks or becomes blocked by a non-wall creature</examplecapture>
[Dependent]
public class CombatEncounter : Glyph
{
    public override Nib[] Nibs => [Prop(Encounter), Prop(Other)];

    public CombatEncounterKind Encounter { get; set; }
    public OneOf<IndefinitePermanent, OneOrMorePermanents> Other { get; set; }
}
