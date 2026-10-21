namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost with a mana alternative: "discard a card or pay {2}", "reveal a goblin card from your hand or pay {3}", "sacrifice a creature or pay {3}{b}". The first cost is any cost glyph.</summary>
/// <exampledoc>Lightning Axe</exampledoc>
/// <examplecapture>discard a card or pay {5}</examplecapture>
[Dependent]
public class CostOrPayMana : Glyph
{
    public override Nib[] Nibs => [Prop(Cost), "or pay", Prop(Mana)];

    public DynamicGlyph Cost { get; set; }
    public ManaCost Mana { get; set; }
}
