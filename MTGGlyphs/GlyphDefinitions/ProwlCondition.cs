namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Prowl's condition: "if you dealt combat damage to a player this turn with a goblin or rogue".</summary>
/// <exampledoc>Auntie's Snitch</exampledoc>
/// <examplecapture>if you dealt combat damage to a player this turn with a goblin or rogue</examplecapture>
[Dependent]
public class ProwlCondition : Glyph
{
    public override Nib[] Nibs => ["if you dealt combat damage to a player this turn with", Pattern("an?"), Prop(Types)];

    public OneOf<ManyOf<CreatureType>, CreatureType?> Types { get; set; }
}
