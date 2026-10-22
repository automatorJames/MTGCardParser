namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The rider on a colored cost reduction: "this effect reduces only the amount of colored/red mana you pay" - generic mana in the cost isn't reduced.</summary>
/// <exampledoc>Edgewalker</exampledoc>
/// <examplecapture>this effect reduces only the amount of colored mana you pay</examplecapture>
public class ReducesOnlyColoredMana : Glyph
{
    public override Nib[] Nibs => ["this effect reduces only the amount of", Prop(Color), "mana you pay"];

    public OneOf<ManaColor?, ManaColorClass?> Color { get; set; }
}
