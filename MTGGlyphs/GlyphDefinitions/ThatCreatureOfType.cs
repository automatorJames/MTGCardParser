namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that wall", "those vampires", "that dragon", "the sliver": refers back to a creature type - a <see cref="CreatureType"/> marked [Referent] earlier in the line, such as the wall met in combat ("becomes blocked by a wall, destroy that wall") or a target of that type.</summary>
/// <exampledoc>Battering Ram</exampledoc>
/// <examplecapture>that wall</examplecapture>
[Dependent]
public class ThatCreatureOfType : BackReference<CreatureType>
{
    public override Nib[] Nibs => [Alt("that", "those", "the"), Plural(Prop(CreatureType))];

    public CreatureType CreatureType { get; set; }
}
