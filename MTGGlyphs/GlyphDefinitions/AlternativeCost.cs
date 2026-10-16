namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost: "you may sacrifice two mountains rather than pay this spell's mana cost", "you may pay {1}{w} rather than pay this spell's mana cost".</summary>
/// <exampledoc>Fireblast</exampledoc>
/// <examplecapture>you may sacrifice two mountains rather than pay this spell's mana cost</examplecapture>
public class AlternativeCost : Glyph
{
    public override Nib[] Nibs => ["you may", Opt("pay"), Prop(Cost), Prop(Also), "rather than pay", Alt("this spell's", "its"), "mana cost"];

    public Cost Cost { get; set; }
    [Optional]
    public AndCost Also { get; set; }
}
