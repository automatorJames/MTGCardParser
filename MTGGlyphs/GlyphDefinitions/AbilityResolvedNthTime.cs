namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which resolution of this ability this turn is happening, as a condition: "this is the third time this ability has resolved this turn", "this is the first time this ability has resolved this turn", and the shorthand of the lines that follow, "it's the second time".</summary>
/// <exampledoc>Inner-Flame Igniter</exampledoc>
/// <examplecapture>this is the third time this ability has resolved this turn</examplecapture>
[Dependent]
public class AbilityResolvedNthTime : Glyph
{
    public override Nib[] Nibs => [Alt("this is", "it's"), "the", Prop(Ordinal), "time", Prop(ThisAbilityThisTurn)];

    public SpellOrdinal Ordinal { get; set; }
    [Optional]
    [RegexPattern("this ability has resolved this turn")]
    public bool ThisAbilityThisTurn { get; set; }
}
