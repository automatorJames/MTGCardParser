namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell narrowed by a mana symbol in its cost: "with {x} in its mana cost", "with {h} in its mana cost", "with {x} in their mana costs".</summary>
/// <exampledoc>Matterbending Mage</exampledoc>
/// <examplecapture>with {x} in its mana cost</examplecapture>
[Dependent]
public class SymbolInManaCost : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Symbol), "in", Alt("its mana cost", "their mana costs")];

    public ManaCost Symbol { get; set; }
}
