namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A creature named by the part it played in the combat event a trigger describes: "the blocking creature", "the attacking creature" (flanking, No Quarter, Righteous Indignation).</summary>
/// <exampledoc>No Quarter</exampledoc>
/// <examplecapture>the attacking creature</examplecapture>
[Dependent]
public class CombatantInEvent : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Role), Plural("creature")];

    public CombatState Role { get; set; }
}
