namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative mana cost for casting a card: "by paying {w}{u}{b}{r}{g} rather than paying its mana cost", "by paying {1} rather than paying its mana cost".</summary>
/// <exampledoc>Raffine's Guidance</exampledoc>
/// <examplecapture>by paying {2}{w} rather than paying its mana cost</examplecapture>
[Dependent]
public class ByPayingInstead : Glyph
{
    public override Nib[] Nibs => ["by paying", Prop(Mana), "rather than paying its mana cost"];

    public ManaCost Mana { get; set; }
}
