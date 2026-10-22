namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost replaced for a group of spells rather than this one: "the mana cost for spells you cast", "the mana cost for zombie creature spells you cast".</summary>
/// <exampledoc>Fist of Suns</exampledoc>
/// <examplecapture>the mana cost for spells you cast</examplecapture>
[Dependent]
public class CostForSpells : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Kind), "cost for", Prop(Spells)];

    public ReplaceableCostKind Kind { get; set; }
    public SpellsCast Spells { get; set; }
}
